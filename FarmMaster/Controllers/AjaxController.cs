using Business.Model;
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
    public class AjaxController : Controller
    {
        #region Animal.Characteristic
        [HttpPost]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.VIEW_ANIMALS)]
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
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.EDIT_ANIMALS)]
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
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.EDIT_ANIMALS)]
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
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.USE_LIFE_EVENT_ENTRY)]
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

        #region Animal.Group
        [HttpPost]
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.EDIT_ANIMALS)]
        public IActionResult Animal_ById_AssignGroup_ById(
            [FromBody] AjaxByIdForIdRequest model,
            User _,
            [FromServices] IServiceAnimalManager animals,
            [FromServices] IServiceAnimalGroupManager groups
        )
        {
            var animal = animals.FromIdAllIncluded(model.ById ?? -1);
            if (animal == null)
                throw new IndexOutOfRangeException($"No animal with ID #{model.ById}");

            var group = groups.Query()
                              .Include(g => g.Animals)
                              .FirstOrDefault(g => g.AnimalGroupId == model.ForId);
            if (group == null)
                throw new IndexOutOfRangeException($"No group with ID #{model.ForId}");

            groups.AssignAnimal(group, animal);

            return new EmptyResult();
        }

        [HttpPost]
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.EDIT_ANIMALS)]
        public IActionResult Animal_ById_RemoveFromGroup_ById(
            [FromBody] AjaxByIdForIdRequest model,
            User _,
            [FromServices] IServiceAnimalManager animals,
            [FromServices] IServiceAnimalGroupManager groups
        )
        {
            var animal = animals.FromIdAllIncluded(model.ById ?? -1);
            if (animal == null)
                throw new IndexOutOfRangeException($"No animal with ID #{model.ById}");

            var group = groups.Query()
                              .Include(g => g.Animals)
                              .FirstOrDefault(g => g.AnimalGroupId == model.ForId);
            if(group == null)
                throw new IndexOutOfRangeException($"No group with ID #{model.ForId}");

            var result = groups.RemoveFromGroup(group, animal);
            if(result == CouldDelete.No)
                throw new InvalidOperationException($"Animal does not belong to group '{group.Name}'");

            return new EmptyResult();
        }
        #endregion

        #region AnimalGroup.Script
        [HttpPost]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.USE_GROUP_SCRIPTS)]
        public IActionResult AnimalGroup_Script_CreateAndCompile_AsName(
            [FromBody] AjaxByIdWithLargeValueRequest model,
            User _,
            [FromServices] IServiceAnimalGroupScriptManager scripts
        ) 
        {
            var script = scripts.CompileAndCreate(model.Value);
            return new AjaxValueResult(new
            {
                name = script.Name
            });
        }

        /// <summary>
        /// This should technically be under the AnimalGroup section, not AnimalGroup.Script,
        /// but AnimalGroup.Script is where most of its uses are going to come from.
        /// </summary>
        [HttpPost]
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.EDIT_ANIMAL_GROUPS)]
        public IActionResult AnimalGroup_ById_Script_AddAllAnimals_ById(
            [FromBody] AjaxByIdWithListRequest<int> model,
            User _,
            [FromServices] IServiceAnimalGroupManager groups,
            [FromServices] IServiceAnimalManager animals
        )
        {
            var group = groups.Query()
                              .Include(g => g.Animals)
                              .FirstOrDefault(g => g.AnimalGroupId == model.Id);
            if (group == null)
                throw new IndexOutOfRangeException($"No group with ID #{model.Id}");

            var query = animals.Query()
                               .Where(a => model.List.Contains(a.AnimalId))
                               .Where(a => !group.Animals.Any(m => m.AnimalId == a.AnimalId))
                               .ToList(); // EF doesn't like iterating a lazy query and saving at the same time.

            foreach(var animal in query)
                groups.AssignAnimal(group, animal); // TODO: Add a SaveChanges parameter to this function.

            return new EmptyResult();
        }

        [HttpPost]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.USE_GROUP_SCRIPTS)]
        public IActionResult AnimalGroup_Script_ByName_Execute_AsNameIdImageId(
            [FromBody] AjaxGroupScriptByNameExecuteRequest model,
            User _,
            [FromServices] IServiceAnimalGroupScriptManager scripts
        )
        {
            var script = scripts.Query().FirstOrDefault(s => s.Name == model.ScriptName);
            if(script == null)
                throw new KeyNotFoundException($"No script called '{model.ScriptName}' exists.");

            return new AjaxValueResult(
                scripts.ExecuteScriptByName(model.ScriptName, model.Parameters)
                       .Include(a => a.Groups)
                       .Where(a => model.AnimalGroupId == null || !a.Groups.Any(g => g.AnimalGroupId == model.AnimalGroupId))
                       .Select(a => new 
                       {
                           name = a.Name,
                           id = a.AnimalId,
                           imageId = a.ImageId
                       })
            );
        }

        [HttpPost]
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.USE_GROUP_SCRIPTS)]
        public IActionResult AnimalGroup_Script_ByName_Delete(
            [FromBody] AjaxByNameRequest model,
            User _,
            [FromServices] IServiceAnimalGroupScriptManager scripts
        )
        {
            var script = scripts.Query().FirstOrDefault(s => s.Name == model.Name);
            if (script == null)
                throw new KeyNotFoundException($"No script called '{model.Name}' exists.");

            scripts.FullDelete(script);
            return new EmptyResult();
        }

        [HttpPost]
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.USE_GROUP_SCRIPTS)]
        public IActionResult AnimalGroup_ById_Script_ExecuteSingleUse_AsNameIdImageId(
            [FromBody] AjaxByIdWithLargeValueRequest model,
            User _,
            [FromServices] IServiceAnimalGroupManager groups,
            [FromServices] IServiceAnimalGroupScriptManager scripts
        )
        {
            var query = this.ExecuteSingleGetQuery(groups, scripts, model.Id ?? -1, model.Value, out AnimalGroup _);
            return new AjaxValueResult(query.Select(a => new
            {
                name = a.Name,
                id = a.AnimalId,
                imageId = a.ImageId
            }));
        }

        private IQueryable<Animal> ExecuteSingleGetQuery(
            IServiceAnimalGroupManager groups,
            IServiceAnimalGroupScriptManager scripts,
            int animalGroupId, 
            string code,
            out AnimalGroup group
        )
        {
            group = groups.Query()
                          .Include(g => g.Animals)
                          .FirstOrDefault(g => g.AnimalGroupId == animalGroupId);
            if (group == null)
                throw new IndexOutOfRangeException($"No group with ID #{animalGroupId}");

            var query = scripts.ExecuteSingleUseScript(code)
                               .Include(a => a.Groups)
                               .Where(a => !a.Groups.Any(g => g.AnimalGroupId == animalGroupId));

            return query;
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
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.VIEW_CONTACTS)]
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
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.VIEW_CONTACTS)]
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
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.EDIT_CONTACTS)]
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
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.EDIT_CONTACTS)]
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
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.VIEW_CONTACTS)]
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
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.EDIT_CONTACTS)]
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
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.EDIT_CONTACTS)]
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
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.VIEW_CONTACTS)]
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
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.EDIT_CONTACTS)]
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
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.EDIT_CONTACTS)]
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
        [FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.VIEW_SPECIES_BREEDS)]
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
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.EDIT_SPECIES_BREEDS)]
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
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.EDIT_SPECIES_BREEDS)]
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
        [FarmAjaxReturnsMessageAndValue(permsAND: BusinessConstants.Permissions.VIEW_SPECIES_BREEDS)]
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
        [FarmAjaxReturnsMessage(permsAND: BusinessConstants.Permissions.EDIT_SPECIES_BREEDS)]
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
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.EDIT_SPECIES_BREEDS)]
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
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.EDIT_LIFE_EVENTS)]
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
        [FarmAjaxReturnsMessage(BusinessConstants.Permissions.EDIT_LIFE_EVENTS)]
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
}