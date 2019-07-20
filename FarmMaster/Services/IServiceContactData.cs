using Business.Model;
using FarmMaster.Misc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceContactData
    {
        Contact ContactFromId(int id);
        void AddTelephoneNumber(Contact contact, User responsible, string reason, string name, string number);
        bool RemoveTelephoneNumberByName(Contact contact, User responsible, string reason, string name);
        void AddEmailAddress(Contact contact, User myUser, string reason, string name, string value);
        bool RemoveEmailAddressByName(Contact contact, User myUser, string reason, string name);
        void LogAction(Contact affected, User responsible, ActionAgainstContactInfo.Type type, string reason, string additionalInfo = null);
        void MakeAnonymous(Contact contact);
    }
    
    public class ServiceContactData : IServiceContactData
    {
        readonly FarmMasterContext _context;

        public ServiceContactData(FarmMasterContext context)
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

        public Contact ContactFromId(int id)
        {
            return this._context.Contacts
                                .Include(c => c.PhoneNumbers)
                                .Include(c => c.EmailAddresses)
                                .Where(c => !c.IsAnonymous)
                                .FirstOrDefault(c => c.ContactId == id);
        }

        public void MakeAnonymous(Contact contact)
        {
            contact = this.ContactFromId(contact.ContactId); // Ensure we have all their data loaded, so we don't miss any.
            contact.IsAnonymous = true;

            foreach(var phone in contact.PhoneNumbers)
                this._context.Remove(phone);

            foreach(var email in contact.EmailAddresses)
                this._context.Remove(email);

            foreach(var relationship in contact.GetRelationships(this._context).ToList()) // ToList, since it's possible removing the entries messes up the original iterator.
                this._context.Remove(relationship);
            
            this._context.SaveChanges();
        }
    }
}
