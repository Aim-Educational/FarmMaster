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
    [FarmAuthorise(PermsOR: new[]{ EnumRolePermission.Names.VIEW_CONTACTS })]
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
                                        .Where(c => !c.IsAnonymous)
            };
            model.ParseMessageQueryString(message);

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.DELETE_CONTACTS })]
        public IActionResult Delete(int id)
        {
            var contactDb = this._contacts.ContactFromId(id);
            if (contactDb == null)
            {
                return RedirectToAction(
                    nameof(Index),
                    new { message = ViewModelWithMessage.CreateMessageQueryString(ViewModelWithMessage.Type.Error, $"Contact with ID #{id} not found.") }
                );
            }

            if(contactDb.ContactType == Contact.Type.User)
            {
                return RedirectToAction(
                    nameof(Index),
                    new { message = ViewModelWithMessage.CreateMessageQueryString(ViewModelWithMessage.Type.Error, "User contacts cannot be deleted by another user.") }
                );
            }

            this._contacts.MakeAnonymous(contactDb);
            return RedirectToAction(nameof(Index));
        }

        [FarmAuthorise(PermsAND: new[]{ EnumRolePermission.Names.EDIT_CONTACTS })]
        public IActionResult Edit(int id, [FromQuery] string reason)
        {
            var contactDb = this._contacts.ContactFromId(id);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_CONTACTS })]
        public IActionResult Create(ContactCreateViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(ModelState));
                return View(model);
            }

            var contact = new Contact
            {
                ContactType = model.Type,
                FullName = model.FullName
            };

            this._context.Add(contact);
            this._contacts.AddEmailAddress(
                contact,
                this._users.UserFromCookieSession(HttpContext),
                "The user is required to specify an email when creating a new contact.",
                GlobalConstants.DefaultNumberName,
                model.Email
            );
            this._context.SaveChanges();

            return RedirectToAction(nameof(Edit), new{ id = contact.ContactId, reason = "The user needs to edit your information after creation." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[]{ EnumRolePermission.Names.EDIT_CONTACTS })]
        public IActionResult Edit(ContactEditViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(ModelState));
                return View(model);
            }

            var contactDb = this._contacts.ContactFromId(model.Contact.ContactId);
            contactDb.FullName = model.Contact.FullName;
            this._context.SaveChanges();

            this._contacts.LogAction(
                contactDb,
                this._users.UserFromCookieSession(HttpContext),
                ActionAgainstContactInfo.Type.Edit_ContactInfo_General,
                "User was not prompted for reason.",
                $"FullName={model.Contact.FullName}"
            );

            return RedirectToAction(nameof(Index));
        }

        #region AJAX
        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxAddPhoneNumber([FromBody] ContactAjaxAddPhoneNumber model)
        {
            return this.DoAjaxWithMessageResponse(model, this._users, this._roles, new[] { EnumRolePermission.Names.EDIT_CONTACTS },
            (myUser) =>
            {
                var contact = this._context.Contacts.Include(c => c.PhoneNumbers).First(c => c.ContactId == model.Id);
                if (contact == null)
                    throw new Exception($"The contact with id #{model.Id} does not exist.");

                if (contact.PhoneNumbers.Any(n => n.Name == model.Name))
                    throw new Exception("There is already a phone number using that name.");
                if (contact.PhoneNumbers.Any(n => n.Number == model.Value))
                    throw new Exception("That phone number is already in use.");

                this._contacts.AddTelephoneNumber(contact, myUser, model.Reason, model.Name, model.Value);
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxRemovePhoneNumberByName([FromBody] ContactAjaxRemoveByName model)
        {
            return this.DoAjaxWithMessageResponse(model, this._users, this._roles, new[] { EnumRolePermission.Names.EDIT_CONTACTS },
            (myUser) =>
            {
                var contact = this._context.Contacts.Include(c => c.PhoneNumbers).First(c => c.ContactId == model.Id);
                if (contact == null)
                    throw new Exception($"The contact with id #{model.Id} does not exist.");

                var couldDelete = this._contacts.RemoveTelephoneNumberByName(contact, myUser, model.Reason, model.Name);
                if (!couldDelete)
                    throw new Exception($"No phone number called '{model.Name}' was found.");
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxAddEmailAddress([FromBody] ContactAjaxAddEmailAddress model)
        {
            return this.DoAjaxWithMessageResponse(model, this._users, this._roles, new[] { EnumRolePermission.Names.EDIT_CONTACTS },
            (myUser) =>
            {
                var contact = this._context.Contacts.Include(c => c.EmailAddresses).First(c => c.ContactId == model.Id);
                if (contact == null)
                    throw new Exception($"The contact with id #{model.Id} does not exist.");

                if (contact.EmailAddresses.Any(n => n.Name == model.Name))
                    throw new Exception("There is already an email using that name.");
                if (contact.EmailAddresses.Any(n => n.Address == model.Value))
                    throw new Exception("That email address is already in use.");

                this._contacts.AddEmailAddress(contact, myUser, model.Reason, model.Name, model.Value);
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxRemoveEmailAddressByName([FromBody] ContactAjaxRemoveByName model)
        {
            return this.DoAjaxWithMessageResponse(model, this._users, this._roles, new[] { EnumRolePermission.Names.EDIT_CONTACTS },
            (myUser) =>
            {
                var contact = this._context.Contacts.Include(c => c.EmailAddresses).First(c => c.ContactId == model.Id);
                if (contact == null)
                    throw new Exception($"The contact with id #{model.Id} does not exist.");
                if (contact.EmailAddresses.Count() == 1)
                    throw new Exception($"Contacts must have at least one email address. You cannot delete the last one.");

                var couldDelete = this._contacts.RemoveEmailAddressByName(contact, myUser, model.Reason, model.Name);
                if (!couldDelete)
                    throw new Exception($"No email address called '{model.Name}' was found.");
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxAddRelation([FromBody] ContactAjaxAddRelationship model)
        {
            return this.DoAjaxWithMessageResponse(model, this._users, this._roles, new[] { EnumRolePermission.Names.EDIT_CONTACTS },
            (myUser) =>
            {
                var contactOne = this._contacts.ContactFromId(model.Id);
                if (contactOne == null)
                    throw new Exception($"The contact with id #{model.Id} does not exist.");

                var contactTwo = this._contacts.ContactFromId(Convert.ToInt32(model.Value));
                if (contactTwo == null)
                    throw new Exception($"The contact with id #{model.Value} does not exist.");

                this._contacts.AddRelationship(contactOne, contactTwo, myUser, model.Reason, model.Name);
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AjaxRemoveRelationById([FromBody] ContactAjaxRemoveByName model)
        {
            return this.DoAjaxWithMessageResponse(model, this._users, this._roles, new[] { EnumRolePermission.Names.EDIT_CONTACTS },
            (myUser) =>
            {
                var contact = this._context.Contacts.Include(c => c.EmailAddresses).First(c => c.ContactId == model.Id);
                if (contact == null)
                    throw new Exception($"The contact with id #{model.Id} does not exist.");

                var couldDelete = this._contacts.RemoveRelationshipById(contact, myUser, model.Reason, Convert.ToInt32(model.Name));
                if (!couldDelete)
                    throw new Exception($"No relationship with id #{model.Name} was found.");
            });
        }
        #endregion
    }
}