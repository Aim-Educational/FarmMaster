using Business.Model;
using FarmMaster.Misc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceContactManager : IServiceEntityManager<Contact>, IServiceGdprData
    {
        Contact Create(Contact.Type type, string fullName, SaveChanges saveChanges = SaveChanges.Yes);
        Contact GetContactFromTokenString(string tokenString);

        Telephone AddTelephoneNumber(Contact contact, User responsible, string reason, string name, string number);
        Email AddEmailAddress(Contact contact, User responsible, string reason, string name, string value);
        MapContactRelationship AddRelationship(Contact first, Contact second, User responsible, string reason, string description);
        ContactToken GenerateToken(Contact contact, ContactToken.Type type, DateTimeOffset expires, IsUnique isUnique = IsUnique.Yes);
        ContactUnsubscribeToken GenerateUnsubscribeToken(string email, DateTimeOffset expires);

        bool RemoveTelephoneNumberByName(Contact contact, User responsible, string reason, string name);
        bool RemoveTelephoneNumberById(Contact contact, User responsible, string reason, int id);
        bool RemoveEmailAddressByName(Contact contact, User responsible, string reason, string name);
        bool RemoveEmailAddressById(Contact contact, User responsible, string reason, int id);
        bool RemoveRelationshipById(Contact contact, User responsible, string reason, int id);
        CouldDelete ExpireTokenByTokenString(Contact contact, string token);

        void LogAction(Contact affected, User responsible, ActionAgainstContactInfo.Type type, string reason, string additionalInfo = null);
        TokenActionResult HandleUnsubscribeFromTokenString(string token);
    }
    
    public class ServiceContactManager : IServiceContactManager
    {
        readonly FarmMasterContext _context;

        public ServiceContactManager(FarmMasterContext context)
        {
            this._context = context;
        }

        public Telephone AddTelephoneNumber(Contact contact, User responsible, string reason, string name, string number)
        {
            var item = new Telephone
            {
                Name = name,
                Contact = contact,
                Number = number
            };

            this._context.Add(item);
            this._context.SaveChanges();

            this.LogAction(
                contact, 
                responsible, 
                ActionAgainstContactInfo.Type.Add_PhoneNumber, 
                reason, 
                $"{name}={number}"
            );

            return item;
        }

        public bool RemoveTelephoneNumberByName(Contact contact, User responsible, string reason, string name)
        {
            var number = contact.PhoneNumbers.FirstOrDefault(p => p.Name == name);
            if(number == null)
                return false;

            this.RemoveTelephoneImpl(contact, responsible, reason, number);
            return true;
        }

        public bool RemoveTelephoneNumberById(Contact contact, User responsible, string reason, int id)
        {
            var number = contact.PhoneNumbers.FirstOrDefault(p => p.TelephoneId == id);
            if(number == null)
                return false;

            this.RemoveTelephoneImpl(contact, responsible, reason, number);
            return true;
        }

        private void RemoveTelephoneImpl(Contact contact, User responsible, string reason, Telephone phone)
        {
            this._context.Telephones.Remove(phone);
            this._context.SaveChanges();

            this.LogAction(
                contact,
                responsible,
                ActionAgainstContactInfo.Type.Delete_PhoneNumber,
                reason,
                $"{phone.Name}={phone.Number}"
            );
        }

        public void LogAction(Contact affected, User responsible, ActionAgainstContactInfo.Type type, string reason, string additionalInfo = null)
        {
            var action = new ActionAgainstContactInfo
            {
                ActionType              = type,
                AdditionalInfo          = additionalInfo ?? "N/A",
                ContactAffected         = affected,
                DateTimeUtc             = DateTimeOffset.UtcNow,
                HasContactBeenInformed  = false,
                Reason                  = reason,
                UserResponsible         = responsible
            };

            this._context.Add(action);
            this._context.SaveChanges();
        }

        public Email AddEmailAddress(Contact contact, User responsible, string reason, string name, string address)
        {
            var item = new Email
            {
                Address = address,
                Contact = contact,
                Name = name
            };

            this._context.Add(item);
            this._context.SaveChanges();

            this.LogAction(
                contact,
                responsible,
                ActionAgainstContactInfo.Type.Add_EmailAddress,
                reason,
                $"{name}={address}"
            );

            return item;
        }

        public bool RemoveEmailAddressByName(Contact contact, User responsible, string reason, string name)
        {
            var email = contact.EmailAddresses.FirstOrDefault(p => p.Name == name);
            if (email == null)
                return false;

            this.RemoveEmailImpl(contact, responsible, reason, email);
            return true;
        }

        public bool RemoveEmailAddressById(Contact contact, User responsible, string reason, int id)
        {
            var email = contact.EmailAddresses.FirstOrDefault(p => p.EmailId == id);
            if (email == null)
                return false;

            this.RemoveEmailImpl(contact, responsible, reason, email);
            return true;
        }

        private void RemoveEmailImpl(Contact contact, User responsible, string reason, Email email)
        {
            this._context.Remove(email);
            this._context.SaveChanges();

            this.LogAction(
                contact,
                responsible,
                ActionAgainstContactInfo.Type.Delete_EmailAddress,
                reason,
                $"{email.Name}={email.Address}"
            );
        }

        public MapContactRelationship AddRelationship(Contact first, Contact second, User responsible, string reason, string description)
        {
            var relation = new MapContactRelationship
            {
                ContactOne = first,
                ContactTwo = second,
                Description = description
            };

            this._context.Add(relation);
            this._context.SaveChanges();

            this.LogAction(
                first, 
                responsible, 
                ActionAgainstContactInfo.Type.Add_Relation, 
                reason,
                $"{description}: {second.ShortName}"
            );

            this.LogAction(
                second, 
                responsible, 
                ActionAgainstContactInfo.Type.Add_Relation, 
                reason,
                $"{description}: {first.ShortName}"
            );

            return relation;
        }

        public bool RemoveRelationshipById(Contact contact, User responsible, string reason, int id)
        {
            var relation = contact.GetRelationships(this._context).FirstOrDefault(r => r.MapContactRelationshipId == id);
            if(relation == null)
                return false;

            this.LogAction(
                relation.ContactOne,
                responsible,
                ActionAgainstContactInfo.Type.Delete_Relation,
                reason,
                $"{relation.Description}: {relation.ContactTwo.ShortName}"
            );

            this.LogAction(
                relation.ContactTwo,
                responsible,
                ActionAgainstContactInfo.Type.Delete_Relation,
                reason,
                $"{relation.Description}: {relation.ContactOne.ShortName}"
            );

            this._context.Remove(relation);
            this._context.SaveChanges();
            return true;
        }

        public IQueryable<Contact> Query()
        {
            return this._context.Contacts
                                .Where(c => !c.IsAnonymous);
        }

        public IQueryable<Contact> QueryAllIncluded()
        {
            return this._context.Contacts
                                .Include(c => c.PhoneNumbers)
                                .Include(c => c.EmailAddresses)
                                .Where(c => !c.IsAnonymous);
        }

        public int GetIdFor(Contact entity)
        {
            return entity.ContactId;
        }

        public Contact Create(Contact.Type type, string fullName, SaveChanges saveChanges = SaveChanges.Yes)
        {
            var contact = new Contact
            {
                ContactType = type,
                FullName = fullName,
                IsAnonymous = false
            };

            this._context.Add(contact);

            if(saveChanges == SaveChanges.Yes)
                this._context.SaveChanges();

            return contact;
        }

        public void Update(Contact entity)
        {
            this._context.Update(entity);
            this._context.SaveChanges();
        }

        public void GetContactGdprData(Contact contact, JObject json)
        {
            json["Contact"] = JObject.FromObject(new 
            {
                ContactType = Convert.ToString(contact.ContactType),
                contact.FullName,
                contact.IsAnonymous,
                Emails = contact.EmailAddresses.Select(e => new { e.Name, e.Address }),
                Phones = contact.PhoneNumbers.Select(p => new { p.Name, p.Number }),
                Relationships = contact.GetRelationships(this._context).Select(r => new
                {
                    r.Description,
                    ContactOneAbbreviatedName = r.ContactOne.FirstNameWithAbbreviatedLastName,
                    ContactTwoAbrreviatedName = r.ContactTwo.FirstNameWithAbbreviatedLastName,
                    Note = "Both contacts have their full names stored, but to protect the contact that isn't you, the names are abbreviated"
                })
            });

            json["ActionsAgainstContacts"] = JArray.FromObject( 
                this._context
                            .ActionsAgainstContactInfo
                            .Where(a => a.UserResponsible.Contact == contact || a.ContactAffected == contact)
                            .Include(a => a.ContactAffected)
                            .Include(a => a.UserResponsible)
                            .ThenInclude(u => u.Contact)
                            .Select(a => new
                            {
                                ActionType = Convert.ToString(a.ActionType),
                                a.AdditionalInfo,
                                AffectedAbbreviatedName = a.ContactAffected.FirstNameWithAbbreviatedLastName,
                                a.DateTimeUtc,
                                a.HasContactBeenInformed,
                                a.Reason,
                                ResponsibleAbbreviatedName = a.UserResponsible.Contact.FirstNameWithAbbreviatedLastName
                            })
            );
        }

        public void GetUserGdprData(User user, JObject json)
        {
            this.GetContactGdprData(user.Contact, json);
        }

        public void AnonymiseContactData(Contact contact)
        {
            Contract.Assert(contact != null);
            Contract.Requires(this._context.Entry(contact).State != EntityState.Detached);

            contact.ContactType = Contact.Type.N_A;
            contact.FullName = "Anonymous Account";
            contact.IsAnonymous = true;
            
            foreach(var email in contact.EmailAddresses)
                this._context.Remove(email);

            foreach(var phone in contact.PhoneNumbers)
                this._context.Remove(phone);

            foreach(var relationship in contact.GetRelationships(this._context))
                this._context.Remove(relationship);

            this._context.SaveChanges();
        }

        public void AnonymiseUserData(User user)
        {
            Contract.Assert(user != null);
            this.AnonymiseContactData(user.Contact);
        }

        public ContactToken GenerateToken(Contact contact, ContactToken.Type type, DateTimeOffset expires, IsUnique isUnique = IsUnique.Yes)
        {
            Contract.Assert(contact != null);

            if(isUnique == IsUnique.Yes)
            {
                foreach(var existingToken in contact.Tokens.Where(t => t.UsageType == type))
                    this._context.Remove(existingToken);
            }

            var token = new ContactToken 
            {
                Contact = contact,
                Expiry = expires,
                Token = Guid.NewGuid().ToString(),
                UsageType = type
            };
            
            this._context.Add(token);
            this._context.SaveChanges();
            return token;
        }

        ContactUnsubscribeToken IServiceContactManager.GenerateUnsubscribeToken(string email, DateTimeOffset expires)
        {
            var token = new ContactUnsubscribeToken 
            {
                Address      = email,
                ExpiresUtc = expires,
                Token      = Guid.NewGuid().ToString()
            };

            this._context.Add(token);
            this._context.SaveChanges();
            return token;
        }

        public CouldDelete ExpireTokenByTokenString(Contact contact, string tokenString)
        {
            Contract.Assert(contact != null);
            
            var token = contact.Tokens.FirstOrDefault(t => t.Token == tokenString);
            if(token == null)
                return CouldDelete.No;

            this._context.Remove(token);
            this._context.SaveChanges();
            return CouldDelete.Yes;
        }

        public Contact GetContactFromTokenString(string tokenString)
        {
            var token = this._context.ContactTokens.FirstOrDefault(t => t.Token == tokenString);
            if(token == null)
                return null;

            return this.FromIdAllIncluded(token.ContactId);
        }

        public TokenActionResult HandleUnsubscribeFromTokenString(string token)
        {
            var dbToken = this._context.ContactUnsubscribeTokens.FirstOrDefault(t => t.Token == token);
            if(dbToken == null)
                return TokenActionResult.Failed;

            if(dbToken.ExpiresUtc >= DateTimeOffset.UtcNow)
                return TokenActionResult.Expired;

            foreach(var contact in this.Query()
                                       .Include(c => c.EmailAddresses)
                                       .Where(c => c.EmailAddresses.Any(a => a.Address == dbToken.Address))
            )
            {
                this._context.Add(new ContactUnsubscribeEntry
                {
                    Address = dbToken.Address,
                    Contact = contact
                });
            }

            this._context.Remove(dbToken);
            this._context.SaveChanges();

            return TokenActionResult.Success;
        }
    }
}
