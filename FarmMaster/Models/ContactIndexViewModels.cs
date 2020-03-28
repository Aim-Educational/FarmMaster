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

    public class ContactCreateViewModel
    {
        public Contact Contact { get; set; }
    }
}
