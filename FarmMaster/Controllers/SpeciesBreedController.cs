using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Filters;
using FarmMaster.Misc;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmMaster.Controllers
{
    [FarmAuthorise(PermsAND: new[]{ EnumRolePermission.Names.VIEW_SPECIES_BREEDS })]
    public class SpeciesBreedController : Controller, IPagingController<Species>, IPagingController<Breed>
    {
        readonly IServiceUserManager _users;
        readonly IServiceRoleManager _roles;
        readonly IServiceSpeciesBreedManager _speciesBreeds;
        readonly IViewRenderService _viewRenderer;

        public SpeciesBreedController(
            IServiceUserManager users, 
            IServiceRoleManager roles, 
            IServiceSpeciesBreedManager speciesBreeds,
            IViewRenderService viewRenderer
        )
        {
            this._users = users;
            this._roles = roles;
            this._speciesBreeds = speciesBreeds;
            this._viewRenderer = viewRenderer;
        }

        public IActionResult Index()
        {
            return View(new SpeciesBreedIndexViewModel
            {
                Species = this._speciesBreeds.For<Species>().Query(),
                Breeds = this._speciesBreeds.For<Breed>().Query()
            });
        }

        #region Species
        public IActionResult CreateSpecies()
        {
            return View();
        }
        
        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_SPECIES_BREEDS })]
        public IActionResult EditSpecies(int id)
        {
            var species = this._speciesBreeds.For<Species>().FromId(id);
            if(species == null)
                return RedirectToAction(nameof(Index));

            return View(new SpeciesEditViewModel{ Species = species });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_SPECIES_BREEDS })]
        public IActionResult CreateSpecies(SpeciesCreateViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(ModelState));
                return View(model);
            }

            var species = this._speciesBreeds.CreateSpecies(model.Name, model.IsPoultry);
            return RedirectToAction(nameof(EditSpecies), new { id = species.SpeciesId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_SPECIES_BREEDS })]
        public IActionResult EditSpecies(SpeciesEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(ModelState));
                return View(model);
            }

            this._speciesBreeds.Update(model.Species);

            model.MessageType = ViewModelWithMessage.Type.Information;
            model.Message = "Success";
            return View(model);
        }
        #endregion

        #region AJAX
        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxGetTablePageCount([FromBody] AjaxPagingControllerRequestModel model)
        {
            return this.DoAjaxWithValueAndMessageResponse(
               model, this._users, this._roles, new string[] { EnumRolePermission.Names.VIEW_SPECIES_BREEDS },
               (myUser) =>
               {
                   var itemCount = (model.EntityType != null && model.EntityType.ToUpper() == "SPECIES")
                                   ? this._speciesBreeds.For<Species>().Query().Count()
                                   : this._speciesBreeds.For<Breed>().Query().Count();

                   return new AjaxStructReturnValue<int>(PagingHelper.CalculatePageCount(itemCount, model.ItemsPerPage));
               }
           );
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxRenderTablePage([FromBody] AjaxPagingControllerRenderRequestModel model)
        {
            return this.DoAjaxWithValueAndMessageResponse(
               model, this._users, this._roles, new string[] { EnumRolePermission.Names.VIEW_SPECIES_BREEDS },
               (myUser) =>
               {
                   if (model.EntityType != null && model.EntityType.ToUpper() == "SPECIES")
                   {
                       return this._viewRenderer.RenderToStringAsync(
                           "/Views/SpeciesBreed/_IndexTableSpeciesBodyPartial.cshtml",
                           this._speciesBreeds.For<Species>().QueryAllIncluded()
                       ).Result;
                   }
                   else
                   {
                       throw new NotImplementedException();
                   }
               }
            );
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxGetCharacteristics([FromBody] AjaxCharacteristicsRequest model)
        {
            return this.DoAjaxWithValueAndMessageResponse(
               model, this._users, this._roles, new string[] { EnumRolePermission.Names.VIEW_SPECIES_BREEDS },
               (myUser) =>
               {
                   if(model.Type == "Species")
                   {
                       var species = this._speciesBreeds.FromIdAllIncluded<Species>(model.Id);
                       if(species == null)
                           throw new NullReferenceException("species");
                       
                       return species.CharacteristicList
                                     .Characteristics
                                     .Select(c => new AjaxCharacteristicsResponseValue{ Name = c.Name, Value = c.Data.ToHtmlString(), Type = (int)c.CalculatedType });
                   }
                   else
                       throw new NotImplementedException(model.Type);
               }
            );
        }
        #endregion
    }
}