using Business.Model;
using FarmMaster.Misc;
using GroupScript;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceAnimalGroupScriptManager : IServiceEntityManager<AnimalGroupScript>,
                                                        IServiceEntityManagerFullDeletion<AnimalGroupScript>,
                                                        IServiceHookConsumer<HookAnimalCreated>
    {
        AnimalGroupScript CompileAndCreate(string code);
        void EditCodeByName(string name, string code);
        IQueryable<Animal> ExecuteScriptByName(string name, IDictionary<string, object> parameters = null);
        IQueryable<Animal> ExecuteSingleUseScript(string code, IDictionary<string, object> parameters = null);
        AnimalGroupScriptAutoEntry CreateAutomatedScript(AnimalGroup group, AnimalGroupScript script, IDictionary<string, object> parameters);
    }

    public class ServiceAnimalGroupScriptManager : IServiceAnimalGroupScriptManager
    {
        readonly FarmMasterContext                        _context;
        readonly IServiceAnimalGroupManager               _groups;
        readonly ILogger<ServiceAnimalGroupScriptManager> _logger;

        public ServiceAnimalGroupScriptManager(
            FarmMasterContext db, 
            IServiceAnimalGroupManager groups,
            ILogger<ServiceAnimalGroupScriptManager> logger
        )
        {
            this._context = db;
            this._groups  = groups;
            this._logger  = logger;
        }

        public AnimalGroupScript CompileAndCreate(string code)
        {
            var parser = new GroupScriptParser(code);
            var ast    = new GroupScriptNodeTree(parser);

            if(this._context.AnimalGroupScripts.Any(s => s.Name == ast.ScriptName))
                throw new InvalidOperationException($"A script called '{ast.ScriptName}' already exists.");

            var script = new AnimalGroupScript()
            {
                Code = code,
                Name = ast.ScriptName
            };

            this._context.Add(script);
            this._context.SaveChanges();

            return script;
        }

        public void EditCodeByName(string name, string code)
        {
            var script = this.GetScriptByName(name);
            var parser = new GroupScriptParser(code);
            var ast = new GroupScriptNodeTree(parser);

            var oldCodeParser = new GroupScriptParser(script.Code);
            var oldCodeAst    = new GroupScriptNodeTree(oldCodeParser);

            if(ast.ScriptName != name)
                throw new InvalidOperationException($"Cannot update code for script '{name}' as the code specified a different name: '{ast.ScriptName}'");

            var entries = this.Query()
                              .Include(s => s.AutomatedScripts)
                              .First(s => s.AnimalGroupScriptId == script.AnimalGroupScriptId)
                              .AutomatedScripts;
            if(entries == null)
                throw new Exception("??");

            // If the script is being used by automation, there's some extra checks we need to perform.
            if(!entries.Any())
            {
                foreach(var param in oldCodeAst.Parameters)
                {
                    if(!ast.Parameters.Any(newParam => newParam.DataType == param.DataType && newParam.Name == param.Name))
                    {
                        throw new InvalidOperationException(
                            $"Cannot update code for script '{name}' as the new code deletes/updates parameter '{param.DataType} {param.Name}' " +
                            $"which is not allowed when the script is being automatically used. Please remove all automated uses of this script " +
                            $"BEFORE changing the parameters of this script."
                        );
                    }
                }
            }

            script.Code = code;

            this._context.Update(script);
            this._context.SaveChanges();
        }

        public IQueryable<Animal> ExecuteScriptByName(string name, IDictionary<string, object> parameters = null)
        {
            var script = this.GetScriptByName(name);
            return this.ExecuteScript(script.Code, parameters);
        }

        public IQueryable<Animal> ExecuteSingleUseScript(string code, IDictionary<string, object> parameters = null)
        {
            return this.ExecuteScript(code, parameters);
        }

        private IQueryable<Animal> ExecuteScript(string code, IDictionary<string, object> parameters)
        {
            var parser     = new GroupScriptParser(code);
            var ast        = new GroupScriptNodeTree(parser);
            var serverCode = GroupScriptCompiler.CompileToStoredProcedureCode(ast, parameters);

            // This is a bit yuck, but something like this was *always* going to be messy in some way.
            return this._context.Animals.FromSql($"SELECT * FROM SP_AnimalGroupScriptFilter({serverCode})");
        }

        public int GetIdFor(AnimalGroupScript entity)
        {
            throw new Exception("Don't use this anymore.");
        }

        public IQueryable<AnimalGroupScript> Query()
        {
            return this._context.AnimalGroupScripts;
        }

        public IQueryable<AnimalGroupScript> QueryAllIncluded()
        {
            throw new Exception("Don't use this anymore.");
        }

        public void Update(AnimalGroupScript entity)
        {
            this._context.Update(entity);

            // In the future, once automated usage of scripts are made, we want to have special checks to ensure
            // the code isn't modified in a way that breaks the automation.
            // 
            // So we want the service to handle this, not any calling code.
            var codeProperty = this._context.Entry(entity).Property(nameof(AnimalGroupScript.Code));
            if(codeProperty.IsModified)
                throw new InvalidOperationException("The code for scripts cannot be modified outside of this service. Please use .EditCode instead.");

            this._context.SaveChanges();
        }

        public void FullDelete(AnimalGroupScript entity)
        {
            this._context.Remove(entity);
            this._context.SaveChanges();
        }

        private AnimalGroupScript GetScriptByName(string name)
        {
            var script = this._context.AnimalGroupScripts.FirstOrDefault(s => s.Name == name);
            if (script == null)
                throw new KeyNotFoundException($"No script called '{name}' was found.");

            return script;
        }

        public AnimalGroupScriptAutoEntry CreateAutomatedScript(AnimalGroup group, AnimalGroupScript script, IDictionary<string, object> parameters)
        {
            // This will ensure that the code and parameters are sound, while also letting us retroactively
            // apply the script.
            var query = this.ExecuteScript(script.Code, parameters);

            var entry = new AnimalGroupScriptAutoEntry 
            {
                AnimalGroup = group,
                AnimalGroupScript = script,
                Parameters = JObject.Parse(JsonConvert.SerializeObject(parameters))
            };

            this._context.Add(entry);
            this._context.SaveChanges();

            foreach(var animal in query)
                this._groups.AssignAnimal(group, animal);

            return entry;
        }

        public void ConsumeHook(HookAnimalCreated hookData)
        {
            var query = this._groups.Query()
                                    .Include(g => g.Animals)
                                    .Include(g => g.AutomatedScripts)
                                     .ThenInclude(g => g.AnimalGroupScript)
                                    .Where(g => g.AutomatedScripts.Any());
            foreach(var group in query)
            {
                foreach(var script in group.AutomatedScripts)
                {
                    // I could *technically* construct a massive SQL query so we only use one request.
                    // Buuuut, I really, really don't think such a thing is worth it at the moment.
                    // So a million seperate queries it is!
                    var animals = this.ExecuteSingleUseScript(
                        script.AnimalGroupScript.Code, 
                        script.Parameters.ToObject<Dictionary<string, object>>()
                    );

                    if(animals.Any(a => a.AnimalId == hookData.animal.AnimalId))
                    {
                        this._logger.LogInformation(
                            FarmConstants.LoggingEvents.AssignByAutoScript, 
                            "Assigning animal #{AnimalId} ({AnimalName}) to group #{GroupId} ({GroupName}) via " +
                            "automated script #{ScriptId}.",
                            hookData.animal.AnimalId,
                            $"[{hookData.animal.Tag}] {hookData.animal.Name}",
                            group.AnimalGroupId,
                            group.Name,
                            script.AnimalGroupScriptAutoEntryId
                        );
                        this._groups.AssignAnimal(group, hookData.animal);
                    }
                }
            }
        }
    }
}
