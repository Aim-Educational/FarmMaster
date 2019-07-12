using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Filters;
using FarmMaster.Misc;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FarmMaster.Controllers
{
    [FarmAuthorise(PermsOR: new[]{ EnumRolePermissionNames.VIEW_CONTACTS })]
    public class ContactController : Controller
    {
        readonly FarmMasterContext _context;
        readonly IServiceSmtpClient _mail;
        readonly IServiceUserManager _users;
        readonly IServiceContactData _contacts;

        public ContactController(
            FarmMasterContext context,
            IServiceSmtpClient mail,
            IServiceUserManager users,
            IServiceContactData contacts
        )
        {
            this._context = context;
            this._mail = mail;
            this._users = users;
            this._contacts = contacts;
        }

        public IActionResult Index([FromQuery] string message)
        {
            var model = new ContactIndexViewModel
            {
                Contacts = this._context.Contacts
            };
            model.ParseMessageQueryString(message);

            return View(model);
        }

        public IActionResult Edit(int id, [FromQuery] string reason)
        {
            var contactDb = this._context.Contacts.Find(id);
            if(contactDb == null)
            {
                return RedirectToAction(nameof(Index), 
                    new{
                        message = ViewModelWithMessage.CreateMessageQueryString(ViewModelWithMessage.Type.Error, $"Contact with ID #{id} does not exist")
                    });
            }

            if(contactDb.ContactType == Contact.Type.Individual
            || contactDb.ContactType == Contact.Type.User)
            {
                var user = this._users.UserFromCookieSession(HttpContext);

                if(user.Contact == contactDb)
                {
                    this._contacts.LogAction(
                        contactDb,
                        user,
                        ActionAgainstContactInfo.Type.View_ContactInfo,
                        reason
                    );
                }
            }

            return View(new ContactEditViewModel{ Contact = contactDb });
        }
    }
}