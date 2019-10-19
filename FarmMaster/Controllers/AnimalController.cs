﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Filters;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            return View(new AnimalIndexViewModel
            {
                AnimalNames = this._animals.Query()
                                           .Select(a => a.Name)
            });
        }

        public IActionResult CreateEdit(int? id)
        {
            var animal = this._animals.FromIdAllIncluded(id ?? -1);
            return (animal == null)
                   ? View(new AnimalCreateEditViewModel{ IsCreate = true })
                   : View(new AnimalCreateEditViewModel
                   {
                       IsCreate = false,
                       AnimalId = animal.AnimalId,
                       DadId = animal.DadId,
                       MumId = animal.MumId,
                       Name = animal.Name,
                       Sex = animal.Sex,
                       OwnerId = animal.OwnerId,
                       Tag = animal.Tag
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
            if(!breeds.Any())
                ModelState.AddModelError("breeds", "No breed was selected");
            if(breeds.Any() && !breeds.All(b => b.SpeciesId == breeds.First().SpeciesId))
                ModelState.AddModelError("breeds", "Not all breeds belong to the same species.");

            return (model.IsCreate) ? this.Create(model, breeds) : null;
        }

        private IActionResult Create(AnimalCreateEditViewModel model, IEnumerable<Breed> breeds)
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
                this._animals.FromId(model.MumId ?? -1), // -1 = ID that shouldn't normally exist, forcing FromId to return null.
                this._animals.FromId(model.DadId ?? -1)
            );

            foreach(var breed in breeds)
                this._animals.AddBreed(animal, breed);

            return RedirectToActionPermanent("CreateEdit", new { id = animal.AnimalId });
        }
        #endregion
    }
}