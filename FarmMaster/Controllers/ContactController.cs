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
            return View();
        }

        [HttpPost]
        [Authorize(Permissions.Contact.Write)]
        public async Task<IActionResult> Create(ContactCreateViewModel model)
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
                        return View(model);
                    }

                    contact = result.Value;
                    workScope.Commit();
                }

                this._logger.LogInformation(
                    "Contact {Contact} created by {User}",
                    contact.Name,
                    User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                );

                return RedirectToAction("Edit", new { contactId = contact.ContactId });
            }

            return View(model);
        }
    }
}