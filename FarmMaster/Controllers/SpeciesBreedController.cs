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
    [FarmAuthorise(PermsAND: new[]{ BusinessConstants.Permissions.VIEW_SPECIES_BREEDS })]
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

        public IActionResult Index(string message)
        {
            var model = new SpeciesBreedIndexViewModel
            {
                Species = this._speciesBreeds.For<Species>().Query(),
                Breeds = this._speciesBreeds.For<Breed>().Query()
            };
            model.ParseMessageQueryString(message);
            return View(model);
        }

        #region Species
        public IActionResult CreateSpecies()
        {
            return View();
        }
        
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.EDIT_SPECIES_BREEDS })]
        public IActionResult EditSpecies(int id)
        {
            var species = this._speciesBreeds.For<Species>().FromId(id);
            if(species == null)
                return RedirectToAction(nameof(Index));

            return View(new SpeciesEditViewModel{ Species = species });
        }

        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.EDIT_SPECIES_BREEDS })]
        public IActionResult DeleteSpecies(int id)
        {
            var species = this._speciesBreeds.For<Species>().FromIdAllIncluded(id);
            if (species == null)
                return RedirectToAction(nameof(Index));

            if(!species.Breeds.All(b => b.IsSafeToDelete))
            {
                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        message = ViewModelWithMessage.CreateMessageQueryString(
                            ViewModelWithMessage.Type.Error, 
                            "Some of the breeds of this species are still in use. Cannot delete."
                        )
                    }
                );
            }

            this._speciesBreeds.FullDelete(species);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.EDIT_SPECIES_BREEDS })]
        public IActionResult CreateSpecies(SpeciesCreateViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(ModelState));
                return View(model);
            }

            var species = this._speciesBreeds.CreateSpecies(model.Name, model.IsPoultry);
            return RedirectToAction(nameof(EditSpecies), new { id = species.SpeciesId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.EDIT_SPECIES_BREEDS })]
        public IActionResult EditSpecies(SpeciesEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(ModelState));
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

        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.EDIT_SPECIES_BREEDS })]
        public IActionResult EditBreed(int id)
        {
            var breed = this._speciesBreeds.For<Breed>().FromId(id);
            if (breed == null)
                return RedirectToAction(nameof(Index));

            return View(new BreedEditViewModel { Breed = breed });
        }

        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.EDIT_SPECIES_BREEDS })]
        public IActionResult DeleteBreed(int id)
        {
            var breed = this._speciesBreeds.For<Breed>().FromIdAllIncluded(id);
            if (breed == null)
                return RedirectToAction(nameof(Index));

            if (!breed.IsSafeToDelete)
            {
                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        message = ViewModelWithMessage.CreateMessageQueryString(
                            ViewModelWithMessage.Type.Error,
                            "This breed is still in use. Cannot delete."
                        )
                    }
                );
            }

            this._speciesBreeds.FullDelete(breed);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.EDIT_SPECIES_BREEDS })]
        public IActionResult CreateBreed(BreedCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(ModelState));
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
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.EDIT_SPECIES_BREEDS })]
        public IActionResult EditBreed(BreedEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(ModelState));
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
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.VIEW_SPECIES_BREEDS)]
        public IActionResult AjaxGetTablePageCount([FromBody] AjaxPagingControllerRequestModel model, User _)
        {
            var itemCount = (model.EntityType != null && model.EntityType.ToUpper() == "SPECIES")
                            ? this._speciesBreeds.For<Species>().Query().Count()
                            : this._speciesBreeds.For<Breed>().Query().Count();

            return new AjaxValueResult(
                new AjaxStructReturnValue<int>(PagingHelper.CalculatePageCount(itemCount, model.ItemsPerPage))
            );
        }

        [HttpPost]
        [AllowAnonymous]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.VIEW_SPECIES_BREEDS)]
        public IActionResult AjaxRenderTablePage([FromBody] AjaxPagingControllerRenderRequestModel model, User _)
        {
            string result;

            var type = (model.EntityType == null) ? "" : model.EntityType.ToUpper();
            if (type == "SPECIES")
            {
                result = this._viewRenderer.RenderToStringAsync(
                    "/Views/SpeciesBreed/_IndexTableSpeciesBodyPartial.cshtml",
                    this._speciesBreeds.For<Species>().QueryAllIncluded()
                ).Result;
            }
            else if (type == "BREED")
            {
                result = this._viewRenderer.RenderToStringAsync(
                    "/Views/SpeciesBreed/_IndexTableBreedBodyPartial.cshtml",
                    this._speciesBreeds.For<Breed>().QueryAllIncluded()
                ).Result;
            }
            else
            {
                throw new NotImplementedException();
            }

            return new AjaxValueResult(result);
        }
        
#pragma warning disable CA1801 // Unused parameter. 'model' *has* to be there otherwise the AJAX attributes throw an exception.
        [HttpPost]
        [AllowAnonymous]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.VIEW_SPECIES_BREEDS)]
        public IActionResult AjaxGetAllSpecies([FromBody] AjaxRequestModel model, User _)
        {
            return new AjaxValueResult(
                this._speciesBreeds
                    .For<Species>()
                    .Query()
                    .OrderBy(s => s.Name)
                    .Select(s => new ComponentSelectOption { Description = s.Name, Value = $"{s.SpeciesId}" })
            );
        }

        [HttpPost]
        [AllowAnonymous]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.VIEW_SPECIES_BREEDS)]
        public IActionResult AjaxGetAllBreeds([FromBody] AjaxRequestModel model, User _)
        {
            return new AjaxValueResult(
                this._speciesBreeds
                    .For<Breed>()
                    .Query()
                    .OrderBy(s => s.Name)
                    .Select(s => new ComponentSelectOption { Description = s.Name, Value = $"{s.BreedId}" })
            );
        }
#pragma warning restore CA1801
        #endregion
    }
}