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

        public AnimalController(
            IServiceAnimalManager animals,
            IServiceContactManager contact,
            IServiceSpeciesBreedManager speciesBreeds
        )
        {
            this._animals = animals;
            this._contacts = contact;
            this._speciesBreeds = speciesBreeds;
        }

        #region GET
        public IActionResult Index(string message)
        {
            var model = new AnimalIndexViewModel
            {
                AnimalNameIdPairs = this._animals.Query()
                                                 .Select(a => new KeyValuePair<string, int>(a.Name, a.AnimalId))
            };
            model.ParseMessageQueryString(message);
            return View(model);
        }

        public IActionResult Create()
        {
            return View("CreateEdit", new AnimalCreateEditViewModel{ IsCreate = true });
        }

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
                OwnerId     = animal.OwnerId,
                Sex         = animal.Sex,
                SpeciesId   = animal.SpeciesId,
                Tag         = animal.Tag,
                ImageId     = animal.ImageId
            });
        }
        #endregion

        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    ModelState.GetValueOrDefault(nameof(model.MumId))
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
                this._animals.FromId(model.DadId ?? -1)
            );
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

            animal.Dad = this._animals.FromId(model.DadId ?? -1);
            animal.Mum = this._animals.FromId(model.MumId ?? -1);
            animal.Name = model.Name;
            animal.Owner = owner;
            animal.Sex = model.Sex;
            animal.Species = species;
            animal.Tag = model.Tag;

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
            this._animals.SetImageFromForm(animal, model.Image);
            return RedirectToActionPermanent("Edit", new { id = animal.AnimalId });
        }
        #endregion
    }
}