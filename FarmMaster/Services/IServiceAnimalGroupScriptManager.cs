using Business.Model;
using GroupScript;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceAnimalGroupScriptManager : IServiceEntityManager<AnimalGroupScript>,
                                                        IServiceEntityManagerFullDeletion<AnimalGroupScript>
    {
        AnimalGroupScript CompileAndCreate(string code);
        void EditCodeByName(string name, string code);
        IQueryable<Animal> ExecuteScriptByName(string name, IDictionary<string, object> parameters = null);
        IQueryable<Animal> ExecuteSingleUseScript(string code, IDictionary<string, object> parameters = null);
    }

    public class ServiceAnimalGroupScriptManager : IServiceAnimalGroupScriptManager
    {
        readonly FarmMasterContext _context;

        public ServiceAnimalGroupScriptManager(FarmMasterContext db)
        {
            this._context = db;
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

            if(ast.ScriptName != name)
                throw new InvalidOperationException($"Cannot update code for script '{name}' as the code specified a different name: '{ast.ScriptName}'");

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
    }
}
