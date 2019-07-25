using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Mvc;

namespace FarmMaster.Controllers
{
    public class HoldingController : Controller
    {
        readonly FarmMasterContext _context;
        readonly IServiceHoldingData _holdings;
        readonly IServiceContactData _contacts;

        public HoldingController(FarmMasterContext context, IServiceHoldingData holdings, IServiceContactData contacts)
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

        public IActionResult Create()
        {
            return View("CreateEdit", new HoldingCreateEditViewModel
            {
                AllRegistrations = this._context.EnumHoldingRegistrations,
                IsCreate = true
            });
        }

        public IActionResult Edit(int id)
        {
            var holding = this._holdings.FromIdAllIncluded(id);
            if(holding == null)
            { 
                return RedirectToAction(
                    nameof(Index), 
                    new
                    {
                        message = ViewModelWithMessage.CreateMessageQueryString(ViewModelWithMessage.Type.Error, "No contact with that ID was found.")
                    }
                );
            }

            return View("CreateEdit", new HoldingCreateEditViewModel
            {
                AllRegistrations = this._context.EnumHoldingRegistrations,
                Holding = holding,
                SelectedRegistrations = this._context.EnumHoldingRegistrations
                                                     .ToDictionary(
                                                        r => r.InternalName, 
                                                        r => holding.Registrations.Select(r2 => r2.HoldingRegistration).Any(r2 => r2.InternalName == r.InternalName)
                                                      ),
                SelectedRegistrationHerdNumbers = this._context.EnumHoldingRegistrations
                                                               .ToDictionary(
                                                                    r => r.InternalName,
                                                                    r => holding.Registrations
                                                                                .FirstOrDefault(r2 => r2.HoldingRegistration.InternalName == r.InternalName)
                                                                                ?.HerdNumber
                                                               )
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HoldingCreateEditViewModel model)
        {
            model.IsCreate = true;

            foreach(var kvp in model.SelectedRegistrationHerdNumbers.Where(kvp => model.SelectedRegistrations[kvp.Key]))
            {
                if(String.IsNullOrEmpty(kvp.Value))
                    ModelState.AddModelError(kvp.Key, "A herd number must be specified for registered critter types.");
            }

            if(!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(ModelState));
                model.AllRegistrations = this._context.EnumHoldingRegistrations;
                return View("CreateEdit", model);
            }

            var contact = this._contacts.ContactFromId(model.Holding.OwnerContactId);
            if(contact == null)
            {
                model.MessageType = ViewModelWithMessage.Type.Error;
                model.Message = $"No contact with ID of #{model.Holding.OwnerContactId}";
                model.AllRegistrations = this._context.EnumHoldingRegistrations;
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
                this._holdings.AddRegistrationByName(holding, reg, model.SelectedRegistrationHerdNumbers[reg]);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(HoldingCreateEditViewModel model)
        {
            foreach (var kvp in model.SelectedRegistrationHerdNumbers.Where(kvp => model.SelectedRegistrations[kvp.Key]))
            {
                if (String.IsNullOrEmpty(kvp.Value))
                    ModelState.AddModelError(kvp.Key, "A herd number must be specified for registered critter types.");
            }

            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(ModelState));
                model.AllRegistrations = this._context.EnumHoldingRegistrations;
                return View("CreateEdit", model);
            }

            var contact = this._contacts.ContactFromId(model.Holding.OwnerContactId);
            if (contact == null)
            {
                model.MessageType = ViewModelWithMessage.Type.Error;
                model.Message = $"No contact with ID of #{model.Holding.OwnerContactId}";
                model.AllRegistrations = this._context.EnumHoldingRegistrations;
                return View("CreateEdit", model);
            }

            var holdingDb           = this._holdings.FromIdAllIncluded(model.Holding.HoldingId);
            holdingDb.Timestamp     = model.Holding.Timestamp;
            holdingDb.Name          = model.Holding.Name;
            holdingDb.HoldingNumber = model.Holding.HoldingNumber;
            holdingDb.GridReference = model.Holding.GridReference;
            holdingDb.Address       = model.Holding.Address;
            holdingDb.Postcode      = model.Holding.Postcode;

            foreach(var kvp in model.SelectedRegistrations)
            {
                if(!kvp.Value)
                    this._holdings.RemoveRegistrationByName(holdingDb, kvp.Key);
                else
                    this._holdings.AddRegistrationByName(holdingDb, kvp.Key, model.SelectedRegistrationHerdNumbers[kvp.Key]);
            }

            return RedirectToAction(nameof(Edit), new { id = model.Holding.HoldingId });
        }
    }
}