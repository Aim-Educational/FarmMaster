using DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLogic
{
    public interface IContactManager : ICrudAsync<Contact>
    {

    }

    public class ContactManager : DbContextCrud<Contact, FarmMasterContext>, IContactManager
    {
        public ContactManager(FarmMasterContext db) : base(db)
        {

        }
    }
}
