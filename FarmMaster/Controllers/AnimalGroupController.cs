using System;
using System.Collections.Generic;
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
    public class AnimalGroupController : Controller
    {
        readonly IServiceAnimalGroupManager _groups;

        public AnimalGroupController(IServiceAnimalGroupManager groups)
        {
            this._groups = groups;
        }

        [FarmAuthorise(new[] { BusinessConstants.Permissions.VIEW_ANIMAL_GROUPS })]
        public IActionResult Index()
        {
            return View(new AnimalGroupIndexViewModel
            {
                Groups = this._groups.Query()
            });
        }

        #region GET
        [FarmAuthorise(new[] { BusinessConstants.Permissions.CREATE_ANIMAL_GROUPS })]
        public IActionResult Create()
        {
            return View("CreateEdit", new AnimalGroupCreateEditViewModel
            { 
                IsCreate = true
            });
        }

        [FarmAuthorise(new[] { BusinessConstants.Permissions.EDIT_ANIMAL_GROUPS })]
        public IActionResult Edit(int id)
        {
            var group = this._groups.Query()
                                    .Include(g => g.Animals)
                                     .ThenInclude(m => m.Animal)
                                    .FirstOrDefault(g => g.AnimalGroupId == id);
            if(group == null)
                throw new Exception();

            return View("CreateEdit", new AnimalGroupCreateEditViewModel
            {
                Description = group.Description,
                GroupId     = group.AnimalGroupId,
                IsCreate    = false,
                Name        = group.Name,
                Animals     = group.Animals.Select(a => new AnimalGroupAnimalInfoViewModel
                {
                    Id       = a.AnimalId,
                    Name     = a.Animal.Name,
                    ImageId  = a.Animal.ImageId
                })
            });
        }
        #endregion

        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(new[]{ BusinessConstants.Permissions.CREATE_ANIMAL_GROUPS })]
        public IActionResult Create(AnimalGroupCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(ModelState));
                return View("CreateEdit", model);
            }

            var group = this._groups.Create(model.Name, model.Description);
            return RedirectToActionPermanent("Edit", new { id = group.AnimalGroupId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(new[]{ BusinessConstants.Permissions.EDIT_ANIMAL_GROUPS })]
        public IActionResult Edit(AnimalGroupCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(ModelState));
                return View("CreateEdit", model);
            }

            var group = this._groups.Query().FirstOrDefault(g => g.AnimalGroupId == model.GroupId);
            if (group == null)
                throw new Exception();

            group.Name = model.Name;
            group.Description = model.Description;
            this._groups.Update(group);

            return View("CreateEdit", model);
        }
        #endregion
    }
}