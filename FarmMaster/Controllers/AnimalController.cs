using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Filters;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FarmMaster.Controllers
{
    public class AnimalController : Controller
    {
        readonly IServiceAnimalManager _animals;
        readonly IServiceContactManager _contacts;
        readonly IServiceSpeciesBreedManager _speciesBreeds;
        readonly IServiceLifeEventManager _lifeEvents;
        readonly IServiceHoldingManager _holdings;

        public AnimalController(
            IServiceAnimalManager animals,
            IServiceContactManager contact,
            IServiceSpeciesBreedManager speciesBreeds,
            IServiceLifeEventManager lifeEvents,
            IServiceHoldingManager holdings
        )
        {
            this._animals       = animals;
            this._contacts      = contact;
            this._speciesBreeds = speciesBreeds;
            this._lifeEvents    = lifeEvents;
            this._holdings      = holdings;
        }

        #region GET
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.VIEW_ANIMALS })]
        public IActionResult Index(string message)
        {
            var model = new AnimalIndexViewModel();
            model.ParseMessageQueryString(message);
            return View(model);
        }

        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.CREATE_ANIMALS })]
        public IActionResult Create()
        {
            return View("CreateEdit", new AnimalCreateEditViewModel{ IsCreate = true });
        }

        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.EDIT_ANIMALS })]
        public IActionResult Edit(int? id)
        {
            if(id == null)
                return RedirectToAction("Index", new { message = ViewModelWithMessage.CreateErrorQueryString("No ID was given.") });

            var animal = this._animals.Query()
                                      .Include(a => a.Breeds)
                                      .FirstOrDefault(a => a.AnimalId == id);
            if(animal == null)
                return RedirectToAction("Index", new { message = ViewModelWithMessage.CreateErrorQueryString($"Animal with ID #{id} does not exist") });

            return View("CreateEdit", new AnimalCreateEditViewModel
            { 
                AnimalId    = id,
                BreedIds    = animal.Breeds.Select(b => b.BreedId),
                DadId       = animal.DadId,
                MumId       = animal.MumId,
                IsCreate    = false,
                Name        = animal.Name,
                OwnerId     = animal.OwnerId ?? -1,
                Sex         = animal.Sex,
                SpeciesId   = animal.SpeciesId,
                Tag         = animal.Tag,
                ImageId     = animal.ImageId,
                HoldingId   = animal.HoldingId
            });
        }

        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.DELETE_ANIMALS })]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("Index", new { message = ViewModelWithMessage.CreateErrorQueryString("No ID was given.") });

            var animal = this._animals.Query().FirstOrDefault(a => a.AnimalId == id);
            if (animal == null)
                return RedirectToAction("Index", new { message = ViewModelWithMessage.CreateErrorQueryString($"Animal with ID #{id} does not exist") });

            try
            { 
                this._animals.FullDelete(animal);
            }
            catch(InvalidOperationException ex)
            {
                return RedirectToAction("Index", new { message = ViewModelWithMessage.CreateErrorQueryString(ex.Message) });
            }

            return RedirectToAction("Index", new { message = ViewModelWithMessage.CreateInfoQueryString("Succesfully deleted animal.") });
        }
        #endregion

        #region POST
        [HttpPost]
        [FarmAuthorise(PermsOR: new[] { BusinessConstants.Permissions.CREATE_ANIMALS, BusinessConstants.Permissions.EDIT_ANIMALS })]
        public IActionResult CreateEdit(AnimalCreateEditViewModel model)
        {
            var breeds = this._speciesBreeds.For<Breed>()
                                            .Query()
                                            .Where(b => model.BreedIds.Any(id => id == b.BreedId))
                                            .Include(b => b.Mappings);

            var species = this._speciesBreeds.For<Species>().FromId(model.SpeciesId ?? -1);

            if(species == null)
                ModelState.AddModelError("species", "That species does not exist.");

            if(breeds.Any() && species != null && !breeds.All(b => b.Species == species))
                ModelState.AddModelError("breeds", "Not all breeds belong to the animal's species.");

            // Because the inputs are set up in a weird way, 'null' values are just empty strings.
            // Model binding doesn't like this too much, so we have to improvise and tell the binder it's ok.
            foreach(var value in new[] 
                { 
                    ModelState.GetValueOrDefault(nameof(model.BreedIds)),
                    ModelState.GetValueOrDefault(nameof(model.DadId)),
                    ModelState.GetValueOrDefault(nameof(model.MumId)),
                    ModelState.GetValueOrDefault(nameof(model.HoldingId))
                }
            )
            {
                if(((string)value.RawValue).Length == 0 && value.ValidationState == ModelValidationState.Invalid)
                    value.ValidationState = ModelValidationState.Valid;
            }

            return (model.IsCreate) ? this.Create(model, species, breeds) : this.Edit(model, species, breeds);
        }

        private IActionResult Create(AnimalCreateEditViewModel model, Species species, IEnumerable<Breed> breeds)
        {
            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(ModelState));
                return View(model);
            }

            var owner = this._contacts.FromId(model.OwnerId);
            if (owner == null)
                throw new Exception($"No owner with the ID #{model.OwnerId}");

            var animal = this._animals.Create(
                model.Name,
                model.Tag,
                model.Sex,
                owner,
                species,
                this._animals.FromId(model.MumId ?? -1), // -1 = ID that shouldn't normally exist, forcing FromId to return null.
                this._animals.FromId(model.DadId ?? -1),
                this._holdings.FromId(model.HoldingId ?? -1)
            );

            if(model.Image != null)
                this._animals.SetImageFromForm(animal, model.Image);

            foreach(var breed in breeds)
                this._animals.AddBreed(animal, breed);

            return RedirectToActionPermanent("Edit", new { id = animal.AnimalId });
        }
        private IActionResult Edit(AnimalCreateEditViewModel model, Species species, IEnumerable<Breed> breeds)
        {
            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(ModelState));
                return View(model);
            }

            var owner = this._contacts.FromId(model.OwnerId);
            if (owner == null)
                throw new Exception($"No owner with the ID #{model.OwnerId}");

            var animal = this._animals.Query()
                                      .Include(a => a.Breeds)
                                       .ThenInclude(m => m.Breed)
                                      .Include(a => a.Image)
                                      .FirstOrDefault(a => a.AnimalId == (model.AnimalId ?? -1));
            if(animal == null)
                throw new Exception($"No animal with the ID #{model.AnimalId}");

            animal.DadId     = model.DadId;
            animal.MumId     = model.MumId;
            animal.HoldingId = model.HoldingId;
            animal.Name      = model.Name;
            animal.Owner     = owner;
            animal.Sex       = model.Sex;
            animal.Species   = species;
            animal.Tag       = model.Tag;

            // First, check for new breeds.
            foreach(var breed in breeds)
            {
                if(!animal.Breeds.Any(b => b.BreedId == breed.BreedId))
                    this._animals.AddBreed(animal, breed);
            }

            // Then, remove any existing breeds that aren't to be used anymore.
            var breedsToRemove = animal.Breeds
                                       .Where(b => !model.BreedIds.Any(id => b.BreedId == id))
                                       .ToList(); // We modify the collection, so have to eager load things.
            foreach(var breed in breedsToRemove)
                this._animals.RemoveBreed(animal, breed.Breed);

            this._animals.Update(animal);
            if (model.Image != null)
                this._animals.SetImageFromForm(animal, model.Image);

            return RedirectToActionPermanent("Edit", new { id = animal.AnimalId });
        }
        #endregion

        #region Callbacks
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.USE_LIFE_EVENT_ENTRY, BusinessConstants.Permissions.EDIT_ANIMALS })]
        public IActionResult OnCreateLifeEvent(int lifeEventEntryId, int redirectEntityId)
        {
            var animal = this._animals
                             .Query() 
                             .Include(a => a.LifeEventEntries)
                              .ThenInclude(m => m.LifeEventEntry)
                               .ThenInclude(e => e.LifeEvent)
                             .FirstOrDefault(a => a.AnimalId == redirectEntityId);
            if(animal == null)
                return Redirect("/");

            var entry = this._lifeEvents
                                .For<LifeEventEntry>()
                                .Query()
                                .Include(e => e.LifeEvent)
                                .FirstOrDefault(e => e.LifeEventEntryId == lifeEventEntryId);
            if(entry == null)
                return RedirectToAction("Edit", new { id = redirectEntityId });

            this._animals.AddLifeEventEntry(animal, entry);
            return RedirectToAction("Edit", new { id = redirectEntityId });
        }
        #endregion
    }
}