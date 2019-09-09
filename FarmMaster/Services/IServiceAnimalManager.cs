using Business.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace FarmMaster.Services
{
    public interface IServiceAnimalManager : IServiceEntityManager<Animal>
    {
        Animal Create(string name, string tag, Animal.Gender sex, Contact owner, Animal mum = null, Animal dad = null);
        void AddLifeEventEntry(Animal animal, LifeEventEntry entry);
        void AddBreed(Animal animal, Breed breed);
        void SetBornEventEntry(Animal animal, DateTimeOffset dateTimeBorn);
        DateTimeOffset? GetBornEventEntry(Animal animal);
    }

    public class ServiceAnimalManager : IServiceAnimalManager
    {
        readonly FarmMasterContext _context;
        readonly IServiceLifeEventManager _lifeEvents;

        public ServiceAnimalManager(FarmMasterContext context, IServiceLifeEventManager lifeEvents)
        {
            this._context = context;
            this._lifeEvents = lifeEvents;
        }

        public Animal Create(string name, string tag, Animal.Gender sex, Contact owner, Animal mum = null, Animal dad = null)
        {
            // Yes, I'm assuming their gender.
            if(mum != null && mum.Sex != Animal.Gender.Female)
                throw new InvalidOperationException($"Mum '{mum.Name}' isn't actually Female.");
            if(dad != null && dad.Sex != Animal.Gender.Male)
                throw new InvalidOperationException($"Dad '{dad.Name}' isn't actually Male.");

            var characteristicList = new AnimalCharacteristicList();
            var animal = new Animal
            {
                Name = name,
                Tag = tag,
                Sex = sex,
                Owner = owner,
                Mum = mum,
                Dad = dad,
                Characteristics = characteristicList
            };

            this._context.Add(characteristicList);
            this._context.Add(animal);
            this._context.SaveChanges();

            return animal;
        }

        public void AddBreed(Animal animal, Breed breed)
        {
            var map = new MapBreedToAnimal
            {
                Animal = animal,
                Breed = breed
            };

            this._context.Add(map);
            this._context.SaveChanges();
        }

        public void AddLifeEventEntry(Animal animal, LifeEventEntry entry)
        {
            var map = new MapLifeEventEntryToAnimal
            {
                Animal = animal,
                LifeEventEntry = entry
            };

            this._context.Add(map);
            this._context.SaveChanges();
        }

        public void SetBornEventEntry(Animal animal, DateTimeOffset dateTimeBorn)
        {
            animal = this.Query()
                         .Include(a => a.LifeEventEntries)
                          .ThenInclude(e => e.LifeEventEntry)
                           .ThenInclude(e => e.LifeEvent)
                         .First(a => a.AnimalId == animal.AnimalId);
            
            var entry = this._lifeEvents.FindBornEventEntryOrNull(animal.LifeEventEntries.Select(e => e.LifeEventEntry));
            if(entry == null)
            {
                entry = this._lifeEvents.CreateBornEventEntry(dateTimeBorn);
                this.AddLifeEventEntry(animal, entry);
            }
            else
            {
                this._lifeEvents.UpdateEventEntryFieldValueByName(
                    entry, 
                    LifeEventDynamicFieldInfo.BuiltinNames.BORN_DATE, 
                    new DynamicFieldDateTime { DateTime = dateTimeBorn }
                );
            }
        }

        public DateTimeOffset? GetBornEventEntry(Animal animal)
        {
            animal = this.Query()
                         .Include(a => a.LifeEventEntries)
                          .ThenInclude(e => e.LifeEventEntry)
                           .ThenInclude(e => e.LifeEvent)
                         .First(a => a.AnimalId == animal.AnimalId);

            var entry = this._lifeEvents.FindBornEventEntryOrNull(animal.LifeEventEntries.Select(e => e.LifeEventEntry));

            // ? null : ... doesn't work for some reason. /shrug
            if(entry == null)
                return null;
            else
                return this._lifeEvents.GetDateTimeBorn(entry);
        }

        public int GetIdFor(Animal entity)
        {
            return entity.AnimalId;
        }

        public IQueryable<Animal> Query()
        {
            return this._context.Animals;
        }

        public IQueryable<Animal> QueryAllIncluded()
        {
            return this._context
                       .Animals
                       .Include(a => a.Breeds)
                        .ThenInclude(b => b.Breed)
                       .Include(a => a.Mum)
                       .Include(a => a.Dad)
                       .Include(a => a.Children_MUM)
                       .Include(a => a.Children_DAD)
                       .Include(a => a.LifeEventEntries)
                        .ThenInclude(e => e.LifeEventEntry)
                       .Include(a => a.Owner);
        }

        public void Update(Animal entity)
        {
            this._context.Update(entity);
        }
    }
}
