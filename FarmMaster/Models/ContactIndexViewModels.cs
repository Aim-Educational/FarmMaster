using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class ContactIndexViewModel
    {
        public IEnumerable<Contact> Contacts { get; set; }
    }

    public class ContactCreateEditViewModel
    {
        public bool IsCreate { get; set; }
        public Contact Contact { get; set; }
    }
}
