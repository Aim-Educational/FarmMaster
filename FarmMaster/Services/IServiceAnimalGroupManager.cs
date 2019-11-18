using Business.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    /// <summary>
    /// Manager for animal groups.
    /// 
    /// Covers creation, querying, and deletion.
    /// </summary>
    public interface IServiceAnimalGroupManager : IServiceEntityManager<AnimalGroup>, IServiceEntityManagerFullDeletion<AnimalGroup>
    {
        AnimalGroup Create(string name, string description);
    }

    public class ServiceAnimalGroupManager : IServiceAnimalGroupManager
    {
        readonly FarmMasterContext _context;

        public ServiceAnimalGroupManager(FarmMasterContext context)
        {
            this._context = context;
        }

        public AnimalGroup Create(string name, string description)
        {
            var group = new AnimalGroup 
            {
                Name = name,
                Description = description
            };

            this._context.Add(group);
            this._context.SaveChanges();

            return group;
        }

        public void FullDelete(AnimalGroup entity)
        {
            this._context.Remove(entity);
            this._context.SaveChanges();
        }

        public int GetIdFor(AnimalGroup entity)
        {
            Contract.Requires(entity != null);
            return entity.AnimalGroupId;
        }

        public IQueryable<AnimalGroup> Query()
        {
            return this._context.AnimalGroups;
        }

        [Obsolete("Use .Query and manual .Includes instead.", true)]
        public IQueryable<AnimalGroup> QueryAllIncluded()
        {
            throw new NotSupportedException();
        }

        public void Update(AnimalGroup entity)
        {
            this._context.Update(entity);
            this._context.SaveChanges();
        }
    }
}
