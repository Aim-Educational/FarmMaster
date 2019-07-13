using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceContactData
    {
        void AddTelephoneNumber(Contact contact, User responsible, string reason, string name, string number);
        bool RemoveTelephoneNumberByName(Contact contact, User responsible, string reason, string name);
        void AddEmailAddress(Contact contact, User myUser, string reason, string name, string value);
        bool RemoveEmailAddressByName(Contact contact, User myUser, string reason, string name);
        void LogAction(Contact affected, User responsible, ActionAgainstContactInfo.Type type, string reason, string additionalInfo = null);
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
    }
}
