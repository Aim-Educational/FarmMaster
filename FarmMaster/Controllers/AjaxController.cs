using Business.Model;
using FarmMaster.Filters;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FarmMaster.Controllers
{
    public class AjaxController : Controller
    {
        #region Contact
        [HttpPost]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Roles.VIEW_CONTACTS)]
        public IActionResult Contact_AsNameId_All(
            [FromBody] AjaxRequestModel _,
            User __,
            [FromServices] IServiceContactManager contacts
        )
        {
            return new AjaxValueResult(
                contacts.Query()
                        .Where(c => !c.IsAnonymous)
                        .Select(c => new { Name = c.ShortName, Id = c.ContactId })
                        .ToList()
            );
        }
        #endregion

        #region Contact.PhoneNumber
        [HttpPost]
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
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Roles.EDIT_CONTACTS)]
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

        #region Contact.Email
        [HttpPost]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Roles.VIEW_CONTACTS)]
        public IActionResult Contact_ById_Email_AsNameValueId_All(
            [FromBody] AjaxByIdRequest model,
            User _,
            [FromServices] IServiceContactManager contacts
        )
        {
            return new AjaxValueResult(
                contacts.Query()
                        .Where(c => c.ContactId == model.Id)
                        .Include(c => c.EmailAddresses)
                        .First()
                        .EmailAddresses
                        .Select(p => new AjaxNameValueId(p.Name, p.Address, p.EmailId))
                        .ToList()
            );
        }

        [HttpPost]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Roles.EDIT_CONTACTS)]
        public IActionResult Contact_ById_Email_Add_ReturnsId(
            [FromBody] AjaxByIdWithNameValueAsEmailReasonRequest model,
            User user,
            [FromServices] IServiceContactManager contacts)
        {
            var contact = contacts.Query()
                                  .Include(c => c.EmailAddresses)
                                  .FirstOrDefault(c => c.ContactId == model.Id);
            if (contact == null)
                throw new Exception($"The contact with id #{model.Id} does not exist.");

            if (contact.EmailAddresses.Any(n => n.Name == model.Name))
                throw new Exception("There is already an email using that name.");
            if (contact.EmailAddresses.Any(n => n.Address == model.Value))
                throw new Exception("That email address is already in use.");

            var email = contacts.AddEmailAddress(contact, user, model.Reason, model.Name, model.Value);
            return new AjaxValueResult(email.EmailId);
        }

        [HttpPost]
        [AllowAnonymous]
        [FarmAjaxReturnsMessage(BusinessConstants.Roles.EDIT_CONTACTS)]
        public IActionResult Contact_ById_Email_Delete_ById(
            [FromBody] AjaxByIdForIdWithReasonRequest model,
            User myUser,
            [FromServices] IServiceContactManager contacts)
        {
            var contact = contacts.Query()
                                  .Include(c => c.EmailAddresses)
                                  .FirstOrDefault(c => c.ContactId == model.ById);
            if (contact == null)
                throw new Exception($"The contact with id #{model.ById} does not exist.");
            if (contact.EmailAddresses.Count() == 1)
                throw new Exception($"Contacts must have at least one email address. You cannot delete the last one.");

            var couldDelete = contacts.RemoveEmailAddressById(contact, myUser, model.Reason, model.ForId);
            return new EmptyResult();
        }
        #endregion

        #region Contact.Relationship
        [HttpPost]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Roles.VIEW_CONTACTS)]
        public IActionResult Contact_ById_Relationship_AsNameValueId_All(
            [FromBody] AjaxByIdRequest model,
            User _,
            [FromServices] IServiceContactManager contacts,
            [FromServices] FarmMasterContext db // ;(
        ) 
        {
            return new AjaxValueResult( 
                contacts.Query()
                        .First(c => c.ContactId == model.Id)
                        .GetRelationships(db)
                        .Select(r => new 
                        { 
                            name = r.Description, 
                            id = r.MapContactRelationshipId,
                            value = (r.ContactOne.ContactId == model.Id)
                                    ? r.ContactTwo.ShortName
                                    : r.ContactOne.ShortName
                        })
            );
        }

        [HttpPost]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Roles.EDIT_CONTACTS)]
        public IActionResult Contact_ById_Relationship_Add_ReturnsId(
            [FromBody] AjaxByIdWithNameValueReasonRequest model, 
            User user,
            [FromServices] IServiceContactManager contacts
        )
        {
            var contactOne = contacts.FromIdAllIncluded(model.Id ?? -1);
            if (contactOne == null)
                throw new Exception($"The contact with id #{model.Id} does not exist.");

            var contactTwo = contacts.FromIdAllIncluded(Convert.ToInt32(model.Value));
            if (contactTwo == null)
                throw new Exception($"The contact with id #{model.Value} does not exist.");

            var map = contacts.AddRelationship(contactOne, contactTwo, user, model.Reason, model.Name);
            return new AjaxValueResult(map.MapContactRelationshipId);
        }

        [HttpPost]
        [FarmAjaxReturnsMessage(BusinessConstants.Roles.EDIT_CONTACTS)]
        public IActionResult Contact_ById_Relationship_Delete_ById(
            [FromBody] AjaxByIdForIdWithReasonRequest model,
            User user,
            [FromServices] IServiceContactManager contacts
        )
        {
            var contact = contacts.Query()
                                  .Include(c => c.EmailAddresses)
                                  .FirstOrDefault(c => c.ContactId == model.ById);
            if (contact == null)
                throw new Exception($"The contact with id #{model.ById} does not exist.");

            var couldDelete = contacts.RemoveRelationshipById(contact, user, model.Reason, model.ForId);
            if (!couldDelete)
                throw new Exception($"No relationship with id #{model.ById} was found.");

            return new EmptyResult();
        }
        #endregion
    }
}