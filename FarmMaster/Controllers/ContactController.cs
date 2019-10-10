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
    [FarmAuthorise(PermsOR: new[]{ BusinessConstants.Roles.VIEW_CONTACTS })]
    public class ContactController : Controller, IPagingController<Contact>
    {
        readonly FarmMasterContext _context;
        readonly IServiceSmtpClient _mail;
        readonly IServiceUserManager _users;
        readonly IServiceContactManager _contacts;
        readonly IServiceRoleManager _roles;
        readonly IViewRenderService _viewRenderer;

        public ContactController(
            FarmMasterContext context,
            IServiceSmtpClient mail,
            IServiceUserManager users,
            IServiceContactManager contacts,
            IServiceRoleManager roles,
            IViewRenderService viewRender
        )
        {
            this._context = context;
            this._mail = mail;
            this._users = users;
            this._contacts = contacts;
            this._roles = roles;
            this._viewRenderer = viewRender;
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

        #region GET
        public IActionResult Create()
        {
            return View();
        }

        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Roles.DELETE_CONTACTS })]
        public IActionResult Delete(int id)
        {
            var contactDb = this._contacts.FromIdAllIncluded(id);
            if (contactDb == null)
            {
                return RedirectToAction(
                    nameof(Index),
                    new { message = ViewModelWithMessage.CreateErrorQueryString($"Contact with ID #{id} not found.") }
                );
            }

            if(contactDb.ContactType == Contact.Type.User)
            {
                return RedirectToAction(
                    nameof(Index),
                    new { message = ViewModelWithMessage.CreateErrorQueryString("User contacts cannot be deleted by another user.") }
                );
            }

            this._contacts.AnonymiseContactData(contactDb);
            return RedirectToAction(nameof(Index));
        }

        [FarmAuthorise(PermsAND: new[]{ BusinessConstants.Roles.EDIT_CONTACTS })]
        public IActionResult Edit(int id, [FromQuery] string reason)
        {
            var contactDb = this._contacts.FromIdAllIncluded(id);
            if(contactDb == null)
            {
                return RedirectToAction(nameof(Index), 
                    new{
                        message = ViewModelWithMessage.CreateErrorQueryString($"Contact with ID #{id} does not exist")
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
        #endregion

        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Roles.EDIT_CONTACTS })]
        public IActionResult Create(ContactCreateViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.ParseInvalidModelState(ModelState);
                return View(model);
            }

            var contact = this._contacts.Create(model.Type, model.FullName);
            this._contacts.AddEmailAddress(
                contact,
                this._users.UserFromCookieSession(HttpContext),
                "The user is required to specify an email when creating a new contact.",
                "Default",
                model.Email
            );

            return RedirectToAction(nameof(Edit), new{ id = contact.ContactId, reason = "The user needs to edit your information after creation." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[]{ BusinessConstants.Roles.EDIT_CONTACTS })]
        public IActionResult Edit(ContactEditViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.ParseInvalidModelState(ModelState);
                return View(model);
            }

            var contactDb = this._contacts.FromIdAllIncluded(model.Contact.ContactId);
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
        #endregion

#pragma warning disable CA1801 // Unused parameter. 'model' *has* to be there otherwise the AJAX attributes throw an exception.
        [HttpPost]
        [AllowAnonymous]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Roles.EDIT_CONTACTS)]
        public IActionResult AjaxGetNameAndValueAll([FromBody] AjaxRequestModel model, User _)
        {
            return new AjaxValueResult(
                    this._context.Contacts
                                 .Where(c => !c.IsAnonymous)
                                 .OrderBy(c => c.ShortName)
                                 .Select(c => new ComponentSelectOption{ Description = c.ShortName, Value = Convert.ToString(c.ContactId) })
                                 .ToList()
            );
        }
#pragma warning restore CA1801

        [HttpPost]
        [AllowAnonymous]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Roles.VIEW_CONTACTS)]
        public IActionResult AjaxGetTablePageCount([FromBody] AjaxPagingControllerRequestModel model, User _)
        {
            var pageCount = PagingHelper.CalculatePageCount(this._contacts.Query().Count(), model.ItemsPerPage);

            return new AjaxValueResult(new AjaxStructReturnValue<int>(pageCount));
        }

        [HttpPost]
        [AllowAnonymous]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Roles.VIEW_CONTACTS)]
        public IActionResult AjaxRenderTablePage([FromBody] AjaxPagingControllerRenderRequestModel model, User _)
        {
            return new AjaxValueResult(
                this._viewRenderer.RenderToStringAsync(
                    "/Views/Contact/_IndexTableBodyPartial.cshtml", 
                    PagingHelper.GetPage(this._contacts.Query(), model.PageToRender, model.ItemsPerPage)
                ).Result
            );
        }
    }
}