using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Filters;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View(new AnimalCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AnimalCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(ModelState));
                return View(model);
            }

            var owner = this._contacts.FromId(model.OwnerId);
            if(owner == null)
                throw new Exception($"No owner with the ID #{model.OwnerId}");
            
            this._animals.Create(
                model.Name,
                model.Tag,
                model.Sex,
                owner,
                this._animals.FromId(model.MumId ?? -1), // -1 = ID that shouldn't normally exist, forcing FromId to return null.
                this._animals.FromId(model.DadId ?? -1)
            );

            throw new NotImplementedException();
        }
    }
}