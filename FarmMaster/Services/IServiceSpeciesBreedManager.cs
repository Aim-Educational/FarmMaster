using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using Microsoft.EntityFrameworkCore;

namespace FarmMaster.Services
{
    public interface IServiceSpeciesBreedManager : IServiceEntityManager<Species>, 
                                                   IServiceEntityManager<Breed>,
                                                   IServiceEntityManagerFullDeletion<Species>,
                                                   IServiceEntityManagerFullDeletion<Breed>
    {
        Species CreateSpecies(string name, bool isPoultry);
        Breed CreateBreed(string name, Species species, Contact breedSociety, bool isRegisterable);
    }

    public class ServiceSpeciesBreedManager : IServiceSpeciesBreedManager
    {
        readonly FarmMasterContext _context;

        public ServiceSpeciesBreedManager(FarmMasterContext context)
        {
            this._context = context;
        }

        public Breed CreateBreed(string name, Species species, Contact breedSociety, bool isRegisterable)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            
            if(species == null)
                throw new ArgumentNullException(nameof(species));

            var list = new AnimalCharacteristicList();
            var breed = new Breed
            {
                BreedSociety = breedSociety,
                CharacteristicList = list,
                IsRegisterable = isRegisterable,
                Name = name,
                Species = species
            };

            this._context.Add(list);
            this._context.Add(breed);
            this._context.SaveChanges();
            return breed;
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

        public void FullDelete(Species entity)
        {
            foreach(var breed in entity.Breeds)
                this._context.Remove(breed);

            this._context.Remove(entity);
            this._context.SaveChanges();
        }

        public void FullDelete(Breed entity)
        {
            this._context.Remove(entity);
            this._context.SaveChanges();
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
                                .Include(s => s.Species)
                                 .ThenInclude(s => s.CharacteristicList)
                                  .ThenInclude(c => c.Characteristics)
                                .Include(s => s.CharacteristicList)
                                 .ThenInclude(c => c.Characteristics)
                                .Include(s => s.Mappings);
        }
    }
}
