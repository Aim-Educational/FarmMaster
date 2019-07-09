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

        public ContactController(
            FarmMasterContext context,
            IServiceSmtpClient mail,
            IServiceUserManager users
        )
        {
            this._context = context;
            this._mail = mail;
            this._users = users;
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
                this._mail.SendToWithTemplateAsync(
                    contactDb.EmailAddresses.Select(e => e.Address),
                    EnumEmailTemplateNames.ContactEditAlert,
                    "Your contact information has been viewed or modified",
                    new EmailContactEditAlertViewModel{ Who = user.Contact.FirstName + " " + user.Contact.LastName, Why = reason }
                ).Wait();
            }

            return View(new ContactEditViewModel{ Contact = contactDb });
        }
    }
}