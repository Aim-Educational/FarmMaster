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
        #endregion
    }
}