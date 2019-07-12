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
            var message = new AjaxModelWithMessage();

            if (!ModelState.IsValid)
            {
                message.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(ModelState));
                return Json(message);
            }

            try
            {
                var myUser = this._users.UserFromCookieSession(model.SessionToken);
                var contact = this._context.Contacts.Find(model.ContactId);
                if (myUser == null)
                    throw new Exception("You are not logged in.");
                if (contact == null)
                    throw new Exception($"The contact with id #{model.ContactId} does not exist.");

                if (!this._roles.HasPermission(myUser.Role, EnumRolePermissionNames.EDIT_CONTACTS))
                    throw new Exception($"You do not have permission to do that.");
                if (this._context.Entry(contact).State == EntityState.Detached)
                    throw new Exception("Internal error. contact is not being tracked by EF");

                if(contact.PhoneNumbers.Any(n => n.Name == model.Name))
                    throw new Exception("There is already a phone number using that name.");
                if(contact.PhoneNumbers.Any(n => n.Number == model.Number))
                    throw new Exception("That phone number is already in use.");

                var number = new Telephone
                {
                    Contact = contact,
                    Name = model.Name,
                    Number = model.Number
                };

                this._context.Add(number);
                this._context.SaveChanges();

                this._contacts.LogAction(
                    contact,
                    myUser,
                    ActionAgainstContactInfo.Type.Add_PhoneNumber,
                    model.Reason,
                    $"{model.Name}={model.Number}"
                );
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.MessageType = ViewModelWithMessage.Type.Error;
                return Json(message);
            }

            return Json(message);
        }
        #endregion
    }
}