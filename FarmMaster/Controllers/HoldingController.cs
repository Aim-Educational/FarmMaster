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
    [FarmAuthorise(PermsOR: new[] { BusinessConstants.Permissions.VIEW_HOLDINGS })]
    public class HoldingController : Controller
    {
        readonly FarmMasterContext _context;
        readonly IServiceHoldingManager _holdings;
        readonly IServiceContactManager _contacts;

        public HoldingController(FarmMasterContext context, IServiceHoldingManager holdings, IServiceContactManager contacts)
        {
            this._holdings = holdings;
            this._context = context;
            this._contacts = contacts;
        }

        public IActionResult Index(string message)
        {
            var model = new HoldingIndexViewModel
            {
                Holdings = this._holdings.QueryAllIncluded()
            };

            model.ParseMessageQueryString(message);
            return View(model);
        }

        #region GET
        [FarmAuthorise(PermsOR: new[] { BusinessConstants.Permissions.CREATE_HOLDINGS })]
        public IActionResult Create()
        {
            return View("CreateEdit", new HoldingCreateEditViewModel
            {
                AllRegistrations = this._context.EnumHoldingRegistrations,
                IsCreate = true
            });
        }

        [FarmAuthorise(PermsOR: new[] { BusinessConstants.Permissions.EDIT_HOLDINGS })]
        public IActionResult Edit(int id)
        {
            var holding = this._holdings.FromIdAllIncluded(id);
            if(holding == null)
            { 
                return RedirectToAction(
                    nameof(Index), 
                    new
                    {
                        message = ViewModelWithMessage.CreateErrorQueryString("No contact with that ID was found.")
                    }
                );
            }

            return View("CreateEdit", new HoldingCreateEditViewModel
            {
                // Forgive my sins, o great Linux neckbeard.
                AllRegistrations = this._context.EnumHoldingRegistrations,
                Holding = holding,
                SelectedRegistrations = this._context.EnumHoldingRegistrations
                                                     .ToDictionary(
                                                        r => r.InternalName, 
                                                        r => holding.Registrations
                                                                    .Select(r2 => r2.HoldingRegistration)
                                                                    .Any(r2 => r2.InternalName == r.InternalName)
                                                      ),
                SelectedRegistrationHerdNumbers = this._context
                                                      .EnumHoldingRegistrations
                                                      .ToDictionary(
                                                           r => r.InternalName,
                                                           r => holding.Registrations
                                                                       .FirstOrDefault(r2 => r2.HoldingRegistration.InternalName == r.InternalName)?
                                                                       .HerdNumber
                                                      ),
                SelectedRegistrationsRareBreedNumbers = this._context
                                                            .EnumHoldingRegistrations
                                                            .ToDictionary(
                                                                 r => r.InternalName,
                                                                 r => holding.Registrations
                                                                             .FirstOrDefault(r2 => r2.HoldingRegistration.InternalName == r.InternalName)?
                                                                             .RareBreedNumber
                                                            )
            });
        }

        [FarmAuthorise(PermsOR: new[] { BusinessConstants.Permissions.DELETE_HOLDINGS })]
        public IActionResult Delete(int id)
        {
            var holding = this._holdings.FromId(id);
            if(holding == null)
            {
                return RedirectToAction(nameof(Index),
                    new{ message = ViewModelWithMessage.CreateErrorQueryString($"The holding with the ID #{id} was not found.") }    
                );
            }

            this._holdings.RemoveByReference(holding);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region POST
        [HttpPost]
        [FarmAuthorise(PermsOR: new[] { BusinessConstants.Permissions.CREATE_HOLDINGS })]
        public IActionResult Create(HoldingCreateEditViewModel model)
        {
            model.AllRegistrations = this._context.EnumHoldingRegistrations;
            model.IsCreate = true;

            foreach(var kvp in model.SelectedRegistrationHerdNumbers.Where(kvp => model.SelectedRegistrations[kvp.Key]))
            {
                if(String.IsNullOrEmpty(kvp.Value))
                    ModelState.AddModelError(kvp.Key, "A herd number must be specified for registered critter types.");
            }

            if(!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(ModelState));
                return View("CreateEdit", model);
            }

            var contact = this._contacts.FromIdAllIncluded(model.Holding.OwnerContactId);
            if(contact == null)
            {
                model.MessageType = ViewModelWithMessage.Type.Error;
                model.Message = $"No contact with ID of #{model.Holding.OwnerContactId}";
                return View("CreateEdit", model);
            }

            var holding = this._holdings.Create(
                model.Holding.Name,
                model.Holding.HoldingNumber,
                model.Holding.GridReference,
                model.Holding.Address,
                model.Holding.Postcode,
                contact
            );

            foreach(var reg in model.SelectedRegistrations.Where(kvp => kvp.Value).Select(kvp => kvp.Key))
            {
                this._holdings.AddRegistrationByName(
                    holding,
                    reg, 
                    model.SelectedRegistrationHerdNumbers[reg],
                    model.SelectedRegistrationsRareBreedNumbers[reg]
                );
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [FarmAuthorise(PermsOR: new[] { BusinessConstants.Permissions.EDIT_HOLDINGS })]
        public IActionResult Edit(HoldingCreateEditViewModel model)
        {
            model.AllRegistrations = this._context.EnumHoldingRegistrations;

            foreach (var kvp in model.SelectedRegistrationHerdNumbers.Where(kvp => model.SelectedRegistrations[kvp.Key]))
            {
                if (String.IsNullOrEmpty(kvp.Value))
                    ModelState.AddModelError(kvp.Key, "A herd number must be specified for registered critter types.");
            }

            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(ModelState));
                return View("CreateEdit", model);
            }

            var contact = this._contacts.FromIdAllIncluded(model.Holding.OwnerContactId);
            if (contact == null)
            {
                model.MessageType = ViewModelWithMessage.Type.Error;
                model.Message = $"No contact with ID of #{model.Holding.OwnerContactId}";
                return View("CreateEdit", model);
            }

            var holdingDb            = this._holdings.FromIdAllIncluded(model.Holding.HoldingId);
            holdingDb.Timestamp      = model.Holding.Timestamp;
            holdingDb.Name           = model.Holding.Name;
            holdingDb.HoldingNumber  = model.Holding.HoldingNumber;
            holdingDb.GridReference  = model.Holding.GridReference;
            holdingDb.Address        = model.Holding.Address;
            holdingDb.Postcode       = model.Holding.Postcode;
            holdingDb.OwnerContactId = contact.ContactId;
            this._context.SaveChanges();

            foreach(var kvp in model.SelectedRegistrations)
            {
                if(!kvp.Value)
                    this._holdings.RemoveRegistrationByName(holdingDb, kvp.Key);
                else 
                {
                    var herdNumber = model.SelectedRegistrationHerdNumbers[kvp.Key] ?? "N/A";
                    var rareBreedNumber = model.SelectedRegistrationsRareBreedNumbers[kvp.Key] ?? "N/A";

                    var added = this._holdings.AddRegistrationByName(
                        holdingDb, 
                        kvp.Key,
                        herdNumber,
                        rareBreedNumber
                    );

                    var alreadyExists = !added; // Naming has more clear intent.
                    if(alreadyExists)
                    {
                        var reg = holdingDb.Registrations.First(r => r.HoldingRegistration.InternalName == kvp.Key);
                        reg.HerdNumber = herdNumber;
                        reg.RareBreedNumber = rareBreedNumber;

                        this._holdings.Update(holdingDb);
                    }
                }
            }

            return RedirectToAction(nameof(Edit), new { id = model.Holding.HoldingId });
        }
        #endregion
    }
}