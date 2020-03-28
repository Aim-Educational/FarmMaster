using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Constants;
using DataAccessLogic;
using FarmMaster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FarmMaster.Controllers
{
    public class ContactController : Controller
    {
        readonly IContactManager _contacts;
        readonly IUnitOfWork     _unitOfWork;
        readonly ILogger         _logger;

        public ContactController(
            IContactManager contacts, 
            IUnitOfWork unitOfWork, 
            ILogger<ContactController> logger
        )
        {
            this._contacts   = contacts;
            this._unitOfWork = unitOfWork;
            this._logger     = logger;
        }

        [Authorize(Permissions.Contact.ManageUI)]
        public IActionResult Index()
        {
            return View(new ContactIndexViewModel
            {
                Contacts = this._contacts.Query() // Effectively a .GetAll
            });
        }

        [Authorize(Permissions.Contact.Write)]
        public IActionResult Create()
        {
            return View("CreateEdit", new ContactCreateEditViewModel
            {
                IsCreate = true
            });
        }

        [Authorize(Permissions.Contact.Read)]
        public async Task<IActionResult> Edit(int? contactId)
        {
            var result = await this._contacts.GetByIdAsync(contactId ?? -1);
            if(!result.Succeeded)
                return RedirectToAction("Index", new { error = result.GatherErrorMessages().FirstOrDefault() });

            return View("CreateEdit", new ContactCreateEditViewModel
            {
                Contact  = result.Value,
                IsCreate = false
            });
        }

        [HttpPost]
        [Authorize(Permissions.Contact.Write)]
        public async Task<IActionResult> Create(ContactCreateEditViewModel model)
        {
            if(ModelState.IsValid)
            {
                // Whitelisting values that the user can provide
                var contact = new Contact 
                {
                    Name  = model.Contact.Name,
                    Email = model.Contact.Email,
                    Phone = model.Contact.Phone,
                    Type  = model.Contact.Type
                };

                using(var workScope = this._unitOfWork.Begin("Create Contact"))
                {
                    var result = await this._contacts.CreateAsync(contact);
                    if(!result.Succeeded)
                    {
                        workScope.Rollback("CreateAsync failed.");

                        foreach(var error in result.GatherErrorMessages())
                            ModelState.AddModelError(string.Empty, error);
                        return View("CreateEdit", model);
                    }

                    contact = result.Value;
                    workScope.Commit();
                }

                this._logger.LogInformation(
                    "Contact {Contact} created by {User}",
                    contact.Name,
                    User.FindFirstValue(ClaimTypes.Name)
                );

                return RedirectToAction("Edit", new { contactId = contact.ContactId });
            }

            return View("CreateEdit", model);
        }

        [HttpPost]
        [Authorize(Permissions.Contact.Write)]
        public async Task<IActionResult> Edit(ContactCreateEditViewModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await this._contacts.GetByIdAsync(model.Contact.ContactId);
                if (!result.Succeeded)
                    return RedirectToAction("Index", new { error = result.GatherErrorMessages().FirstOrDefault() });

                var dbContact   = result.Value;
                dbContact.Name  = model.Contact.Name;
                dbContact.Type  = model.Contact.Type;
                dbContact.Phone = model.Contact.Phone;
                dbContact.Email = model.Contact.Email;

                using(var workScope = this._unitOfWork.Begin("Edit Contact"))
                {
                    var updateResult = this._contacts.Update(dbContact);
                    if(!updateResult.Succeeded)
                    {
                        workScope.Rollback("CreateAsync failed.");

                        foreach (var error in result.GatherErrorMessages())
                            ModelState.AddModelError(string.Empty, error);
                        return View("CreateEdit", model);
                    }

                    workScope.Commit();
                }

                this._logger.LogInformation(
                    "Contact {Contact} updated by {User}",
                    dbContact.Name,
                    User.FindFirstValue(ClaimTypes.Name)
                );
            }

            return View("CreateEdit", model);
        }
    }
}