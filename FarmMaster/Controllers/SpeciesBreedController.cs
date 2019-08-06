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
        readonly IServiceCharacteristicManager _characteristics;
        readonly IServiceContactManager _contacts;

        public SpeciesBreedController(
            IServiceUserManager users, 
            IServiceRoleManager roles, 
            IServiceSpeciesBreedManager speciesBreeds,
            IViewRenderService viewRenderer,
            IServiceCharacteristicManager characteristics,
            IServiceContactManager contacts
        )
        {
            this._users = users;
            this._roles = roles;
            this._speciesBreeds = speciesBreeds;
            this._viewRenderer = viewRenderer;
            this._characteristics = characteristics;
            this._contacts = contacts;
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

        #region Breed
        public IActionResult CreateBreed()
        {
            return View();
        }

        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_SPECIES_BREEDS })]
        public IActionResult EditBreed(int id)
        {
            var breed = this._speciesBreeds.For<Breed>().FromId(id);
            if (breed == null)
                return RedirectToAction(nameof(Index));

            return View(new BreedEditViewModel { Breed = breed });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_SPECIES_BREEDS })]
        public IActionResult CreateBreed(BreedCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(ModelState));
                return View(model);
            }
            
            var species = this._speciesBreeds.For<Species>().FromId(model.SpeciesId ?? -1); // Id should never be null because of the model binding checks.
            var contact = this._contacts.FromId(model.BreedSocietyContactId ?? -1);

            if (species == null)
            {
                model.MessageType = ViewModelWithMessage.Type.Error;
                model.Message = $"The species with ID #{model.SpeciesId} could not be found.";
                return View(model);
            }
            if (contact == null)
            {
                model.MessageType = ViewModelWithMessage.Type.Error;
                model.Message = $"The species with ID #{model.BreedSocietyContactId} could not be found.";
                return View(model);
            }

            var breed = this._speciesBreeds.CreateBreed(model.Name, species, contact, model.IsRegisterable);
            return RedirectToAction("EditBreed", new{ id = breed.BreedId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_SPECIES_BREEDS })]
        public IActionResult EditBreed(BreedEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(ModelState));
                return View(model);
            }

            this._speciesBreeds.Update(model.Breed);

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
                   var type = (model.EntityType == null) ? "" : model.EntityType.ToUpper();
                   if (type == "SPECIES")
                   {
                       return this._viewRenderer.RenderToStringAsync(
                           "/Views/SpeciesBreed/_IndexTableSpeciesBodyPartial.cshtml",
                           this._speciesBreeds.For<Species>().QueryAllIncluded()
                       ).Result;
                   }
                   else if(type == "BREED")
                   {
                       return this._viewRenderer.RenderToStringAsync(
                           "/Views/SpeciesBreed/_IndexTableBreedBodyPartial.cshtml",
                           this._speciesBreeds.For<Breed>().QueryAllIncluded()
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
            return (this).DoAjaxWithValueAndMessageResponse(
               model, this._users, this._roles, new string[] { EnumRolePermission.Names.VIEW_SPECIES_BREEDS },
               (myUser) =>
               {
                   var list = this.GetOrCreateListForEntity(model.Type, model.Id);
                   return list.Characteristics
                              .Select(c => new AjaxCharacteristicsResponseValue { Name = c.Name, Value = c.Data.ToHtmlString(), Type = (int)c.DataType });
               }
            );
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxAddCharacteristic([FromBody] AjaxCharacteristicsAddRequest model)
        {
            return this.DoAjaxWithMessageResponse(
               model, this._users, this._roles, new string[] { EnumRolePermission.Names.EDIT_SPECIES_BREEDS },
               (myUser) =>
               {
                   var list = this.GetOrCreateListForEntity(model.EntityType, model.EntityId);
                   var chara = this._characteristics.CreateFromHtmlString(
                       list,
                       model.CharaName,
                       model.CharaType,
                       model.CharaValue
                   );
               }
            );
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxDeleteCharacteristicByName([FromBody] AjaxCharacteristicsDeleteByNameRequest model)
        {
            return this.DoAjaxWithMessageResponse(
               model, this._users, this._roles, new string[] { EnumRolePermission.Names.EDIT_SPECIES_BREEDS },
               (myUser) =>
               {
                   var list = this.GetOrCreateListForEntity(model.EntityType, model.EntityId);
                   var chara = list.Characteristics.FirstOrDefault(c => c.Name == model.CharaName);
                   if(chara == null)
                       throw new KeyNotFoundException(model.CharaName);

                   this._characteristics.FullDelete(chara);
               }
            );
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxGetAllSpecies([FromBody] AjaxRequestModel model)
        {
            return this.DoAjaxWithValueAndMessageResponse(
               model, this._users, this._roles, new string[] { EnumRolePermission.Names.VIEW_SPECIES_BREEDS },
               (myUser) =>
               {
                   return this._speciesBreeds
                              .For<Species>()
                              .Query()
                              .OrderBy(s => s.Name)
                              .Select(s => new ComponentSelectOption{ Description = s.Name, Value = $"{s.SpeciesId}" });
               }
            );
        }
        #endregion

        #region Helpers
        private AnimalCharacteristicList GetOrCreateListForEntity(string type, int id)
        {
            if (type == "Species")
            {
                var species = this._speciesBreeds.For<Species>().FromIdAllIncluded(id);
                if (species == null)
                    throw new NullReferenceException("species");

                return species.CharacteristicList;
            }
            else if(type == "Breed")
            {
                var breed = this._speciesBreeds.For<Breed>().FromIdAllIncluded(id);
                if (breed == null)
                    throw new NullReferenceException("breed");

                return breed.CharacteristicList;
            }
            else
                throw new NotImplementedException(type);
        }
        #endregion
    }
}