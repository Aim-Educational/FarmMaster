using Business.Model;
using FarmMaster.Misc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceContactManager : IServiceEntityManager<Contact>
    {
        Contact Create(Contact.Type type, string fullName, SaveChanges saveChanges = SaveChanges.Yes);

        void AddTelephoneNumber(Contact contact, User responsible, string reason, string name, string number);
        void AddEmailAddress(Contact contact, User responsible, string reason, string name, string value);
        void AddRelationship(Contact first, Contact second, User responsible, string reason, string description);

        bool RemoveTelephoneNumberByName(Contact contact, User responsible, string reason, string name);
        bool RemoveEmailAddressByName(Contact contact, User responsible, string reason, string name);
        bool RemoveRelationshipById(Contact contact, User responsible, string reason, int id);

        void LogAction(Contact affected, User responsible, ActionAgainstContactInfo.Type type, string reason, string additionalInfo = null);
        void MakeAnonymous(Contact contact);
    }
    
    public class ServiceContactManager : IServiceContactManager
    {
        readonly FarmMasterContext _context;

        public ServiceContactManager(FarmMasterContext context)
        {
            this._context = context;
        }

        public void AddTelephoneNumber(Contact contact, User responsible, string reason, string name, string number)
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
        }

        public bool RemoveTelephoneNumberByName(Contact contact, User responsible, string reason, string name)
        {
            var number = contact.PhoneNumbers.FirstOrDefault(p => p.Name == name);
            if(number == null)
                return false;

            this._context.Telephones.Remove(number);
            this._context.SaveChanges();

            this.LogAction(
                contact,
                responsible,
                ActionAgainstContactInfo.Type.Delete_PhoneNumber,
                reason,
                $"{name}={number.Number}"
            );
            return true;
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

        public void AddEmailAddress(Contact contact, User responsible, string reason, string name, string address)
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
        }

        public bool RemoveEmailAddressByName(Contact contact, User responsible, string reason, string name)
        {
            var email = contact.EmailAddresses.FirstOrDefault(p => p.Name == name);
            if (email == null)
                return false;

            this._context.Remove(email);
            this._context.SaveChanges();

            this.LogAction(
                contact,
                responsible,
                ActionAgainstContactInfo.Type.Delete_EmailAddress,
                reason,
                $"{name}={email.Address}"
            );
            return true;
        }

        public void MakeAnonymous(Contact contact)
        {
            contact = this.FromIdAllIncluded(contact.ContactId); // Ensure we have all their data loaded, so we don't miss any.
            contact.IsAnonymous = true;

            foreach(var phone in contact.PhoneNumbers)
                this._context.Remove(phone);

            foreach(var email in contact.EmailAddresses)
                this._context.Remove(email);

            foreach(var relationship in contact.GetRelationships(this._context).ToList()) // ToList, since it's possible removing the entries messes up the original iterator.
                this._context.Remove(relationship);
            
            this._context.SaveChanges();
        }

        public void AddRelationship(Contact first, Contact second, User responsible, string reason, string description)
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
            return this._context.Contacts;
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
    }
}
