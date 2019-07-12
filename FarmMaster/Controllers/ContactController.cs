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
using Microsoft.EntityFrameworkCore;
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
        readonly IServiceRoleManager _roles;

        public ContactController(
            FarmMasterContext context,
            IServiceSmtpClient mail,
            IServiceUserManager users,
            IServiceContactData contacts,
            IServiceRoleManager roles
        )
        {
            this._context = context;
            this._mail = mail;
            this._users = users;
            this._contacts = contacts;
            this._roles = roles;
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

        [FarmAuthorise(PermsAND: new[]{ EnumRolePermissionNames.EDIT_CONTACTS })]
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

                if(user.Contact != contactDb)
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

        #region AJAX
        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxAddPhoneNumber([FromBody] ContactAjaxAddPhoneNumber model)
        {
            return this.DoAjaxWithMessageResponse(model, this._users, this._roles, new[]{ EnumRolePermissionNames.EDIT_CONTACTS }, 
            (myUser) => 
            {
                var contact = this._context.Contacts.Include(c => c.PhoneNumbers).First(c => c.ContactId == model.ContactId);
                if (contact == null)
                    throw new Exception($"The contact with id #{model.ContactId} does not exist.");

                if(contact.PhoneNumbers.Any(n => n.Name == model.Name))
                    throw new Exception("There is already a phone number using that name.");
                if(contact.PhoneNumbers.Any(n => n.Number == model.Value))
                    throw new Exception("That phone number is already in use.");

                this._contacts.AddTelephoneNumber(contact, myUser, model.Reason, model.Name, model.Value);
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxRemovePhoneNumberByName([FromBody] ContactAjaxRemovePhoneNumberByName model)
        {
            return this.DoAjaxWithMessageResponse(model, this._users, this._roles, new[] { EnumRolePermissionNames.EDIT_CONTACTS }, 
            (myUser) => 
            {
                var contact = this._context.Contacts.Include(c => c.PhoneNumbers).First(c => c.ContactId == model.ContactId);
                if (contact == null)
                    throw new Exception($"The contact with id #{model.ContactId} does not exist.");

                var couldDelete = this._contacts.RemoveTelephoneNumberByName(contact, myUser, model.Reason, model.Name);
                if (!couldDelete)
                    throw new Exception($"No phone number called '{model.Name}' was found.");
            });
        }
        #endregion
    }
}