using Business.Model;
using GroupScript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceAnimalGroupScriptManager : IServiceEntityManager<AnimalGroupScript>
    {
        AnimalGroupScript CompileAndCreate(string code);
        IQueryable<Animal> ExecuteSingleUseScript(IQueryable<Animal> query, string code);
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

            var script = new AnimalGroupScript()
            {
                Code = code,
                Name = ast.ScriptName
            };

            this._context.Add(script);
            this._context.SaveChanges();

            return script;
        }

        public IQueryable<Animal> ExecuteSingleUseScript(IQueryable<Animal> query, string code)
        {
            throw new NotImplementedException("TODO");
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
            this._context.SaveChanges();
        }
    }
}
