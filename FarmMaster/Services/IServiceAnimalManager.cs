using Business.Model;
using FarmMaster.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace FarmMaster.Services
{
    public interface IServiceAnimalManager : IServiceEntityManager<Animal>, 
                                             IServiceGdprData,
                                             IServiceEntityManagerFullDeletion<Animal>
    {
        Animal Create(string name, string tag, Animal.Gender sex, Contact owner, Species species, Animal mum = null, Animal dad = null, Holding holding = null);
        void AddLifeEventEntry(Animal animal, LifeEventEntry entry);
        void AddBreed(Animal animal, Breed breed);
        void SetImageFromForm(Animal animal, IFormFile image);
        CouldDelete RemoveBreed(Animal animal, Breed breed);
        CouldDelete RemoveLifeEventEntry(Animal animal, LifeEventEntry entry);
        void SetBornEventEntry(Animal animal, DateTimeOffset dateTimeBorn);
        DateTimeOffset? GetBornEventEntry(Animal animal);
    }

    public class HookAnimalCreated
    {
        public Animal animal { get; set; }
    }

    public class ServiceAnimalManager : IServiceAnimalManager
    {
        readonly FarmMasterContext              _context;
        readonly IServiceLifeEventManager       _lifeEvents;
        readonly IServiceImageManager           _images;
        readonly IServiceHookEmitter            _hooks;
        readonly IServiceAnimalGroupManager     _groups;
        readonly IServiceCharacteristicManager  _characteristics;

        public ServiceAnimalManager(
            FarmMasterContext               context, 
            IServiceLifeEventManager        lifeEvents,
            IServiceImageManager            images,
            IServiceHookEmitter             hooks,
            IServiceAnimalGroupManager      groups,
            IServiceCharacteristicManager   characteristics)
        {
            this._context           = context;
            this._lifeEvents        = lifeEvents;
            this._images            = images;
            this._hooks             = hooks;
            this._groups            = groups;
            this._characteristics   = characteristics;
        }

        public Animal Create(string name, string tag, Animal.Gender sex, Contact owner, Species species, Animal mum, Animal dad, Holding holding)
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
                Characteristics = characteristicList,
                Species = species,
                Holding = holding
            };

            this._context.Add(characteristicList);
            this._context.Add(animal);
            this._context.SaveChanges();

            this._hooks.Emit(new HookAnimalCreated { animal = animal });

            return animal;
        }

        public void AddBreed(Animal animal, Breed breed)
        {
            if(breed.SpeciesId != animal.SpeciesId)
                throw new InvalidOperationException($"Cannot assign breed '{breed.Name}' to animal '{animal.Name}' as the breed does not belong to the animal's species.");

            var map = new MapBreedToAnimal
            {
                Animal = animal,
                Breed = breed
            };

            this._context.Add(map);
            this._context.SaveChanges();
        }

        public void SetImageFromForm(Animal animal, IFormFile formImage)
        {
            Contract.Requires(animal != null);
            Contract.Requires(formImage != null);

            // It's a bit of a weird behaviour to delete the old image instead of updating it.
            // But it actually makes the logic a bit easier to manage, due to not having to duplicate
            // certain logic such as validation checks (UploadFromForm does all that for me).
            if(animal.ImageId != null)
                this._context.Remove(animal.Image);

            var image = this._images.UploadFromForm(formImage).Result;
            animal.ImageId = image.ImageId;

            this.Update(animal);
        }

        public CouldDelete RemoveBreed(Animal animal, Breed breed)
        {
            var map = animal.Breeds.FirstOrDefault(b => b.BreedId == breed.BreedId);
            if(map == null)
                return CouldDelete.No;

            this._context.Remove(map);
            this._context.SaveChanges();
            return CouldDelete.Yes;
        }

        public void AddLifeEventEntry(Animal animal, LifeEventEntry entry)
        {
            if(animal == null)
                throw new ArgumentNullException(nameof(animal));

            if(entry.LifeEvent.Target != LifeEvent.TargetType.Animal)
                throw new InvalidOperationException($"Cannot apply Life Event '{entry.LifeEvent.Name}' that targets '{entry.LifeEvent.Target}' on an Animal.");

            if(entry.LifeEvent.IsUnique
               && animal.LifeEventEntries.Any(e => e.LifeEventEntry.LifeEventId == entry.LifeEventId)
            )
            {
                throw new InvalidOperationException(
                    $"Cannot create entry for life event '{entry.LifeEvent.Name}' as it is unique, and animal '{animal.Name}' already contains an entry for it."
                );
            }

            var map = new MapLifeEventEntryToAnimal
            {
                Animal = animal,
                LifeEventEntry = entry
            };

            this._context.Add(map);
            this._context.SaveChanges();
        }

        public CouldDelete RemoveLifeEventEntry(Animal animal, LifeEventEntry entry)
        {
            var map = animal.LifeEventEntries
                            .FirstOrDefault(e => e.LifeEventEntryId == entry.LifeEventEntryId);
            if(map == null)
                return CouldDelete.No;

            this._context.Remove(map);

            this._context.SaveChanges();
            return CouldDelete.Yes;
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
                    BusinessConstants.BuiltinLifeEventFields.BORN_DATE, 
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
                         .ThenInclude(e => e.LifeEvent)
                       .Include(a => a.Owner)
                       .Include(a => a.Image)
                       .Include(a => a.Characteristics)
                        .ThenInclude(c => c.Characteristics)
                       .Include(a => a.Holding);
        }

        public void Update(Animal entity)
        {
            this._context.Update(entity);
            this._context.SaveChanges();
        }

        public void GetContactGdprData(Contact contact, JObject json)
        {
            json["AnimalsOwned"] = JArray.FromObject(
                this.QueryAllIncluded()
                    .Where(a => a.Owner == contact)
                    .Select(a => $"[{a.Tag}] a.Name")
            );
        }

        public void GetUserGdprData(User user, JObject json)
        {
            this.GetContactGdprData(user.Contact, json);
        }

        public void AnonymiseContactData(Contact contact)
        {}

        public void AnonymiseUserData(User user)
        {}

        // This function can call SaveChanges a million different times.
        // 1. This is a design flaw. Everything that modifies the DB should have a SaveChanges param, I will enforce this after Alpha.
        // 2. This will likely only ever be used on animals that are made by mistake, so there's probably not much to delete anyway.
        public void FullDelete(Animal animal)
        {
            // Saves the callee having to do this, since this is pretty much *everything*.
            animal = this._context
                         .Animals
                         .Include(a => a.Breeds)
                         .Include(a => a.Characteristics)
                          .ThenInclude(c => c.Characteristics)
                         .Include(a => a.Children_DAD)
                         .Include(a => a.Children_MUM)
                         .Include(a => a.Groups)
                          .ThenInclude(m => m.AnimalGroup)
                           .ThenInclude(g => g.Animals)
                         .Include(a => a.Image)
                         .Include(a => a.LifeEventEntries)
                          .ThenInclude(m => m.LifeEventEntry)
                           .ThenInclude(e => e.Values)
                         .Include(a => a.LifeEventEntries)
                          .ThenInclude(m => m.LifeEventEntry)
                           .ThenInclude(e => e.AnimalMap)
                         .First(a => a.AnimalId == animal.AnimalId);

            if(animal.Children.Any())
                throw new InvalidOperationException($"Cannot delete {animal.Name} as they are the parent of at least one other animal.");

            foreach(var breedMap in animal.Breeds.ToList()) // Modifying a collection when iterating it doesn't work, so we ToList it.
                this._context.Remove(breedMap);

            this._context.SaveChanges();                              // SaveChanges #1
            this._characteristics.FullDelete(animal.Characteristics); // SaveChanges #2 O(n) since its a list of characteristics.

            if(animal.Image != null)
                this._images.FullDelete(animal.Image);                // SaveChanges #3

            foreach (var groupMap in animal.Groups.ToList())
                this._groups.RemoveFromGroup(groupMap.AnimalGroup, animal); // SaveChanges #4 O(n)

            foreach(var entryMap in animal.LifeEventEntries.ToList())
            {
                this.RemoveLifeEventEntry( // SaveChanges #5 O(n)
                    animal, 
                    entryMap.LifeEventEntry
                );
            }

            // Hopefully that's everything, otherwise we'd leave a few orphan records laying around, even with cascade delete.
            // This is mostly because things like characteristics and life events can exist seperately of the entity they're attached to.
            // So only the mappings would cascade delete, not the actual entity being mapped to the animal.
            // Reusuability and flexibility in exchange for ease of use ;(
            animal = this.Query().First(a => a.AnimalId == animal.AnimalId); // Otherwise EF complains it contains old data or smth.
            this._context.Remove(animal);
            this._context.SaveChanges(); // SaveChanges #6
        }
    }
}
