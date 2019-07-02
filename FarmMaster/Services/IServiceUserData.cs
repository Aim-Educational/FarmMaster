using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceUserData
    {
        void AddTelephoneNumber(User user, string name, string number);
    }
    
    public class ServiceUserData : IServiceUserData
    {
        readonly FarmMasterContext _context;

        public ServiceUserData(FarmMasterContext context)
        {
            this._context = context;
        }

        public void AddTelephoneNumber(User user, string name, string number)
        {
            var item = new Telephone
            {
                Name = name,
                Contact = user.Contact,
                Number = number
            };

            this._context.Add(item);
            this._context.SaveChanges();
        }
    }
}
