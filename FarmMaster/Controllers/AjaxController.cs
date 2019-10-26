﻿using Business.Model;
using FarmMaster.Filters;
using FarmMaster.Misc;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmMaster.Controllers
{
#pragma warning disable CA1801 // Unused parameters. Usually there due to the AJAX attributes requiring them, even if they're not used.
#pragma warning disable CA1062 // Validate arguments of public methods. Ignored since the only chance of this happening is if we forget to register a service, which is a 1 min fix for all functions.
#pragma warning disable CA1822 // Mark members as static. Ignored since it's incompatible with ASP Core's routing stuff.
#pragma warning disable CA1707 // Identifiers should not contain underscores. Ignored since it's an explicit design choice for AJAX callbacks.
    public class AjaxController : Controller
    {
        #region Animal.Characteristic
        [HttpPost]
        [FarmAjaxReturnsMessageAndValue]
        public IActionResult Animal_ById_Characteristic_AsNameValueTypeInheritedId_All(
            [FromBody] AjaxByIdRequest model,
            User _,
            [FromServices] IServiceAnimalManager animals
        )
        {
            var animal = animals.FromIdAllIncluded(model.Id ?? -1);
            if (animal == null)
                throw new IndexOutOfRangeException($"No animal with ID #{model.Id}");

            var list = animal.Characteristics;
            return new AjaxValueResult(
                list.Characteristics
                    .Select(c => new AjaxCharacteristicsResponseValue
                    {
                        Name = c.Name,
                        Value = c.Data.ToHtmlString(),
                        Type = Enum.GetName(typeof(DynamicField.Type), c.Data.FieldType),
                        IsInherited = false,
                        Id = c.AnimalCharacteristicId
                    })
            );
        }

        [HttpPost]
        [FarmAjaxReturnsMessage]
        public IActionResult Animal_ById_Characteristic_Delete_ById(
            [FromBody] AjaxByIdForIdRequest model,
            User _,
            [FromServices] IServiceAnimalManager animals,
            [FromServices] IServiceCharacteristicManager characteristics
        )
        {
            var animal = animals.FromIdAllIncluded(model.ById ?? -1);
            if (animal == null)
                throw new IndexOutOfRangeException($"No animal with ID #{model.ById}");

            characteristics.FullDeleteById(animal.Characteristics, model.ForId);
            return new EmptyResult();
        }

        [HttpPost]
        [FarmAjaxReturnsMessage]
        public IActionResult Animal_ById_Characteristic_Add(
            [FromBody] AjaxCharacteristicsAddRequest model,
            User _,
            [FromServices] IServiceAnimalManager animals,
            [FromServices] IServiceCharacteristicManager characteristics
        )
        {
            var animal = animals.FromIdAllIncluded(model.Id);
            if (animal == null)
                throw new IndexOutOfRangeException($"No animal with ID #{model.Id}");

            characteristics.CreateFromHtmlString(
                animal.Characteristics,
                model.Name,
                model.Type,
                model.Value
            );

            return new EmptyResult();
        }
        #endregion

        #region Animal.LifeEventEntry
        [HttpPost]
        [FarmAjaxReturnsMessage(permsAND: new[] { BusinessConstants.Roles.EDIT_LIFE_EVENT_ENTRY })]
        public IActionResult Animal_ById_LifeEventEntry_Delete_ById(
            [FromBody] AjaxByIdForIdRequest model,
            User _,
            [FromServices] IServiceAnimalManager animals,
            [FromServices] IServiceLifeEventManager lifeEvents
        )
        {
            var animal = animals.FromIdAllIncluded(model.ById ?? -1);
            if(animal == null)
                throw new IndexOutOfRangeException($"No animal with ID #{model.ById}");

            var entry = lifeEvents.For<LifeEventEntry>().FromId(model.ForId);
            if(entry == null)
                throw new IndexOutOfRangeException($"No life event entry with ID #{model.ForId}");

            var couldDelete = animals.RemoveLifeEventEntry(animal, entry);
            if(couldDelete == CouldDelete.No)
                throw new InvalidOperationException("Unable to delete life event.");

            return new EmptyResult();
        }
        #endregion

        #region Account
        [HttpPost]
        [FarmAjaxReturnsMessage]
        public IActionResult Account_BySession_SendAnonymiseRequest_VerifyByPassword(
            [FromBody] AccountAjaxWithPasswordRequest model,
            User user,
            [FromServices] IServiceContactManager contacts,
            [FromServices] IServiceSmtpClient email,
            [FromServices] IServiceUserManager users,
            [FromServices] IOptions<IServiceSmtpDomainConfig> domains
        )
        {
            if(!users.UserPasswordMatches(user.UserLoginInfo.Username, model.Password))
                throw new Exception("Password is incorrect.");

            var token = contacts.GenerateToken(
                user.Contact,
                ContactToken.Type.Anonymise,
                DateTimeOffset.UtcNow + TimeSpan.FromDays(1),
                IsUnique.Yes
            );

            email.SendToWithTemplateAsync(
                user,
                FarmConstants.EmailTemplateNames.AnonymisationRequest,
                "Confirm request to anonymise a contact/user your email is associated with.",
                (
                    callback: domains.Value.AnonRequest + token.Token,
                    contactName: user.Contact.FullName
                )
            ).Wait();

            return new EmptyResult();
        }
        #endregion

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

        #region Species.Characteristic
        [HttpPost]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Roles.VIEW_SPECIES_BREEDS)]
        public IActionResult Species_ById_Characteristic_AsNameValueTypeInheritedId_All(
            [FromBody] AjaxByIdRequest model, 
            User _,
            [FromServices] IServiceSpeciesBreedManager speciesBreeds
        )
        {            
            var species = speciesBreeds.For<Species>().FromIdAllIncluded(model.Id ?? -1);
            if(species == null)
                throw new IndexOutOfRangeException($"No species with ID #{model.Id}");

            var list = species.CharacteristicList;
            return new AjaxValueResult(
                list.Characteristics
                    .Select(c => new AjaxCharacteristicsResponseValue
                    {
                        Name = c.Name,
                        Value = c.Data.ToHtmlString(),
                        Type = Enum.GetName(typeof(DynamicField.Type), c.Data.FieldType),
                        IsInherited = false,
                        Id = c.AnimalCharacteristicId
                    })
            );
        }

        [HttpPost]
        [FarmAjaxReturnsMessage(BusinessConstants.Roles.EDIT_SPECIES_BREEDS)]
        public IActionResult Species_ById_Characteristic_Delete_ById(
            [FromBody] AjaxByIdForIdRequest model,
            User _,
            [FromServices] IServiceSpeciesBreedManager speciesBreeds,
            [FromServices] IServiceCharacteristicManager characteristics
        )
        {
            var species = speciesBreeds.For<Species>().FromIdAllIncluded(model.ById ?? -1);
            if (species == null)
                throw new IndexOutOfRangeException($"No species with ID #{model.ById}");

            characteristics.FullDeleteById(species.CharacteristicList, model.ForId);
            return new EmptyResult();
        }

        [HttpPost]
        [FarmAjaxReturnsMessage(BusinessConstants.Roles.EDIT_SPECIES_BREEDS)]
        public IActionResult Species_ById_Characteristic_Add(
            [FromBody] AjaxCharacteristicsAddRequest model, 
            User _,
            [FromServices] IServiceSpeciesBreedManager speciesBreeds,
            [FromServices] IServiceCharacteristicManager characteristics
        )
        {
            var species = speciesBreeds.For<Species>().FromIdAllIncluded(model.Id);
            if (species == null)
                throw new IndexOutOfRangeException($"No species with ID #{model.Id}");

            characteristics.CreateFromHtmlString(
                species.CharacteristicList,
                model.Name,
                model.Type,
                model.Value
            );

            return new EmptyResult();
        }
        #endregion

        #region Breed.Characteristic
        [HttpPost]
        [FarmAjaxReturnsMessageAndValue(permsAND: BusinessConstants.Roles.VIEW_SPECIES_BREEDS)]
        public IActionResult Breed_ById_Characteristic_AsNameTypeValueInheritedId_All(
            [FromBody] AjaxByIdRequest model,
            User _,
            [FromServices] IServiceSpeciesBreedManager speciesBreeds
        )
        {
            var breed       = this.GetBreedByIdEnforceExists(model.Id ?? -1, speciesBreeds);
            var speciesList = breed.Species.CharacteristicList;
            var combined    = breed.CharacteristicList.Characteristics.Concat(speciesList.Characteristics);

            return new AjaxValueResult(
                combined.Select(c => new AjaxCharacteristicsResponseValue
                {
                    Name = c.Name,
                    Value = c.Data.ToHtmlString(),
                    Type = Enum.GetName(typeof(DynamicField.Type), c.Data.FieldType),
                    IsInherited = speciesList.Characteristics.Any(sc => sc.AnimalCharacteristicId == c.AnimalCharacteristicId),
                    Id = c.AnimalCharacteristicId
                })
            );
        }

        [HttpPost]
        [FarmAjaxReturnsMessage(permsAND: BusinessConstants.Roles.EDIT_SPECIES_BREEDS)]
        public IActionResult Breed_ById_Characteristic_Delete_ById(
            [FromBody] AjaxByIdForIdRequest model,
            User _,
            [FromServices] IServiceSpeciesBreedManager speciesBreeds,
            [FromServices] IServiceCharacteristicManager characteristics
        )
        {
            var breed = this.GetBreedByIdEnforceExists(model.ById ?? -1, speciesBreeds);
            characteristics.FullDeleteById(breed.CharacteristicList, model.ForId);
            return new EmptyResult();
        }

        [HttpPost]
        [FarmAjaxReturnsMessage(BusinessConstants.Roles.EDIT_SPECIES_BREEDS)]
        public IActionResult Breed_ById_Characteristic_Add(
            [FromBody] AjaxCharacteristicsAddRequest model,
            User _,
            [FromServices] IServiceSpeciesBreedManager speciesBreeds,
            [FromServices] IServiceCharacteristicManager characteristics
        )
        {
            var breed = this.GetBreedByIdEnforceExists(model.Id, speciesBreeds);
            characteristics.CreateFromHtmlString(
                breed.CharacteristicList,
                model.Name,
                model.Type,
                model.Value
            );

            return new EmptyResult();
        }

        private Breed GetBreedByIdEnforceExists(int id, IServiceSpeciesBreedManager speciesBreeds)
        {
            var breed = speciesBreeds.For<Breed>().FromIdAllIncluded(id);
            if (breed == null)
                throw new IndexOutOfRangeException($"No breed with ID #{id}");

            return breed;
        }
        #endregion

        #region LifeEvent.Field
        [HttpPost]
        [FarmAjaxReturnsMessage(BusinessConstants.Roles.EDIT_LIFE_EVENTS)]
        public IActionResult LifeEvent_ById_Field_Add(
            [FromBody] AjaxByIdWithNameValueTypeAsTRequest<DynamicField.Type> model, // 'Value' is the event's description 
            User _,
            [FromServices] IServiceLifeEventManager lifeEvents
        )
        {
            var @event = lifeEvents.For<LifeEvent>().FromIdAllIncluded(model.Id ?? -1);
            if (@event == null)
                throw new IndexOutOfRangeException($"No event with ID #{model.Id} was found.");

            if (@event.IsInUse)
                throw new InvalidOperationException($"Cannot modify the fields of event '{@event.Name}' as it is currently in use.");

            lifeEvents.CreateEventField(@event, model.Name, model.Value, model.Type);
            return new EmptyResult();
        }

        [HttpPost]
        [FarmAjaxReturnsMessage(BusinessConstants.Roles.EDIT_LIFE_EVENTS)]
        public IActionResult LifeEvent_ById_Field_Delete_ByName(
            [FromBody] AjaxByIdWithNameRequest model, 
            User _,
            [FromServices] IServiceLifeEventManager lifeEvents
        )
        {
            var @event = lifeEvents.For<LifeEvent>().FromIdAllIncluded(model.Id ?? -1);
            if (@event == null)
                throw new IndexOutOfRangeException($"No event with ID #{model.Id} was found.");

            if (@event.IsInUse)
                throw new InvalidOperationException($"Cannot modify the fields of event '{@event.Name}' as it is currently in use.");

            var result = lifeEvents.RemoveEventFieldByName(@event, model.Name);
            if (result == CouldDelete.No)
                throw new IndexOutOfRangeException($"Could not delete field '{model.Name}', does it even exist?");

            return new EmptyResult();
        }
        #endregion
    }
#pragma warning restore CA1801 // Unused parameters. Usually there due to the AJAX attributes requiring them, even if they're not used.
#pragma warning restore CA1062 // Validate arguments of public methods
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore CA1707 // Identifiers should not contain underscores
}