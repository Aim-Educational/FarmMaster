using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;

namespace FarmMaster.Models
{
    public class ContactIndexViewModel : ViewModelWithMessage
    {
        public IEnumerable<Contact> Contacts;
    }

    public class ContactEditViewModel : ViewModelWithMessage
    {
        [Required]
        public Contact Contact { get; set; }
    }
}
