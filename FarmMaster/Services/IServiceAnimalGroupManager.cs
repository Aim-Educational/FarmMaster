using Business.Model;
using FarmMaster.Misc;
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
        MapAnimalToAnimalGroup AssignAnimal(AnimalGroup group, Animal animal);
        CouldDelete RemoveFromGroup(AnimalGroup group, Animal animal);
    }

    public class ServiceAnimalGroupManager : IServiceAnimalGroupManager
    {
        readonly FarmMasterContext _context;

        public ServiceAnimalGroupManager(FarmMasterContext context)
        {
            this._context = context;
        }

        public MapAnimalToAnimalGroup AssignAnimal(AnimalGroup group, Animal animal)
        {
            Contract.Requires(group != null);
            Contract.Requires(animal != null);

            if(group.Animals.Any(m => m.AnimalId == animal.AnimalId))
            {
                throw new InvalidOperationException(
                     $"Animal #{animal.AnimalId} '{animal.Name}' is "
                    +$"already assigned to group #{group.AnimalGroupId} '{group.Name}'");
            }

            var map = new MapAnimalToAnimalGroup 
            {
                Animal      = animal,
                AnimalGroup = group
            };

            this._context.Add(map);
            this._context.SaveChanges();

            return map;
        }

        public CouldDelete RemoveFromGroup(AnimalGroup group, Animal animal)
        {
            Contract.Requires(group != null);
            Contract.Requires(animal != null);

            var map = group.Animals.FirstOrDefault(m => m.AnimalId == animal.AnimalId);
            if(map == null)
                return CouldDelete.No;

            this._context.Remove(map);
            this._context.SaveChanges();

            return CouldDelete.Yes;
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
            foreach(var map in this._context.MapAnimalToAnimalGroups.Where(m => m.AnimalGroupId == entity.AnimalGroupId))
                this._context.Remove(map);

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
