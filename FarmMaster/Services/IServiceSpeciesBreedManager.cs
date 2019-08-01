using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using Microsoft.EntityFrameworkCore;

namespace FarmMaster.Services
{
    public interface IServiceSpeciesBreedManager : IServiceEntityManager<Species>, IServiceEntityManager<Breed>
    {
        Species CreateSpecies(string name, bool isPoultry);
        Breed CreateBreed();
    }

    public class ServiceSpeciesBreedManager : IServiceSpeciesBreedManager
    {
        readonly FarmMasterContext _context;

        public ServiceSpeciesBreedManager(FarmMasterContext context)
        {
            this._context = context;
        }

        public Breed CreateBreed()
        {
            throw new NotImplementedException();
        }

        public Species CreateSpecies(string name, bool isPoultry)
        {
            if(String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var list = new AnimalCharacteristicList();

            var species = new Species
            {
                IsPoultry = isPoultry,
                Name = name,
                CharacteristicList = list
            };
            
            this._context.Add(species);
            this._context.Add(list);
            this._context.SaveChanges();
            return species;
        }

        public int GetIdFor(Species entity)
        {
            return entity.SpeciesId;
        }

        public int GetIdFor(Breed entity)
        {
            return entity.BreedId;
        }

        public IQueryable<Species> Query()
        {
            return this._context.Species;
        }

        public IQueryable<Species> QueryAllIncluded()
        {
            return this._context.Species
                                .Include(s => s.Breeds)
                                .Include(s => s.CharacteristicList)
                                 .ThenInclude(c => c.Characteristics);
        }

        public void Update(Species entity)
        {
            this._context.Update(entity);
            this._context.SaveChanges();
        }

        public void Update(Breed entity)
        {
            this._context.Update(entity);
            this._context.SaveChanges();
        }

        IQueryable<Breed> IServiceEntityManager<Breed>.Query()
        {
            return this._context.Breeds;
        }

        IQueryable<Breed> IServiceEntityManager<Breed>.QueryAllIncluded()
        {
            return this._context.Breeds
                                .Include(s => s.SpeciesType)
                                 .ThenInclude(s => s.CharacteristicList)
                                  .ThenInclude(c => c.Characteristics)
                                .Include(s => s.CharacteristicList)
                                 .ThenInclude(c => c.Characteristics);
        }
    }
}
