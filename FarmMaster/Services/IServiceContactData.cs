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
    }
}
