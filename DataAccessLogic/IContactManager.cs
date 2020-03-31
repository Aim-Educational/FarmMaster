using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public override IQueryable<Contact> IncludeAll(IQueryable<Contact> query)
        {
            return query.Include(c => c.NoteOwner)
                         .ThenInclude(o => o.NoteEntries);
        }
    }
}
