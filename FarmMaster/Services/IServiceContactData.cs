using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceContactData
    {
        void AddTelephoneNumber(Contact contact, string name, string number);
    }
    
    public class ServiceContactData : IServiceContactData
    {
        readonly FarmMasterContext _context;

        public ServiceContactData(FarmMasterContext context)
        {
            this._context = context;
        }

        public void AddTelephoneNumber(Contact contact, string name, string number)
        {
            var item = new Telephone
            {
                Name = name,
                Contact = contact,
                Number = number
            };

            this._context.Add(item);
            this._context.SaveChanges();
        }
    }
}
