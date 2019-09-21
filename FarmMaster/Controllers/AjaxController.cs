using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Filters;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmMaster.Controllers
{
    public class AjaxController : Controller
    {
        #region Contact
        [HttpPost]
        [AllowAnonymous]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Roles.VIEW_CONTACTS)]
        public IActionResult Contact_ById_PhoneNumber_AsNameValueId_All(
            [FromBody] AjaxByIdRequest model,
            User _,
            [FromServices] IServiceContactManager contacts
        )
        {
            return new AjaxValueResult(
                contacts.Query()
                        .Where(c => c.ContactId == model.Id)
                        .Include(c => c.PhoneNumbers)
                        .First()
                        .PhoneNumbers
                        .Select(p => new AjaxNameValueId(p.Name, p.Number, p.TelephoneId))
                        .ToList()
            );
        }

        [HttpPost]
        [AllowAnonymous]
        [FarmAjaxReturnsMessage(BusinessConstants.Roles.EDIT_CONTACTS)]
        public IActionResult Contact_ById_PhoneNumber_Delete_ById(
            [FromBody] AjaxByIdForIdWithReasonRequest model, 
            User user, 
            [FromServices] IServiceContactManager contacts
        )
        {
            var contact = contacts.Query()
                                  .Where(c => c.ContactId == model.ById)
                                  .Include(c => c.PhoneNumbers)
                                  .First();

            contacts.RemoveTelephoneNumberById(contact, user, model.Reason, model.ForId);
            return new EmptyResult();
        }

        [HttpPost]
        [AllowAnonymous]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Roles.VIEW_CONTACTS)]
        public IActionResult Contact_ById_PhoneNumber_Add_ReturnsId(
            [FromBody] AjaxByIdWithNameValueReasonRequest model,
            User user,
            [FromServices] IServiceContactManager contacts
        )
        {
            var contact = contacts.Query()
                                  .Include(c => c.PhoneNumbers)
                                  .FirstOrDefault(c => c.ContactId == model.Id);
            if (contact == null)
                throw new Exception($"The contact with id #{model.Id} does not exist.");
            if (contact.PhoneNumbers.Any(n => n.Name == model.Name))
                throw new Exception("There is already a phone number using that name.");
            if (contact.PhoneNumbers.Any(n => n.Number == model.Value))
                throw new Exception("That phone number is already in use.");

            var phone = contacts.AddTelephoneNumber(contact, user, model.Reason, model.Name, model.Value);
            return new AjaxValueResult(phone.TelephoneId);
        }
        #endregion
    }
}