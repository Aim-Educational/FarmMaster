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

        public IActionResult Index()
        {
            return View(new HoldingIndexViewModel
            {
                Holdings = this._holdings.QueryAllIncluded()
            });
        }

        public IActionResult Create()
        {
            return View("CreateEdit", new HoldingCreateEditViewModel
            {
                AllRegistrations = this._context.EnumHoldingRegistrations,
                IsCreate = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HoldingCreateEditViewModel model)
        {
            model.IsCreate = true;

            if(!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(ModelState));
                model.AllRegistrations = this._context.EnumHoldingRegistrations;
                return View(model);
            }

            var contact = this._contacts.ContactFromId(model.Holding.OwnerContactId);
            if(contact == null)
            {
                model.MessageType = ViewModelWithMessage.Type.Error;
                model.Message = $"No contact with ID of #{model.Holding.OwnerContactId}";
                model.AllRegistrations = this._context.EnumHoldingRegistrations;
                return View(model);
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
                this._holdings.AddRegistrationByName(holding, reg);

            return RedirectToAction(nameof(Index));
        }
    }
}