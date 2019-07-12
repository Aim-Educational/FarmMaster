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

    public class ContactAjaxAddPhoneNumber : AjaxModel
    {
        [Required]
        public int ContactId { get; set; }

        [Required(ErrorMessage = "A name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A number is required.")]
        [RegularExpression(@"^\+?[0-9]*$", ErrorMessage = "Invalid Phone Number. Only '+', and digits are allowed.")]
        public string Number { get; set; }

        [Required(ErrorMessage = "You must give a reason for adding this number.")]
        public string Reason { get; set; }
    }

    public class ContactAjaxRemovePhoneNumberByName : AjaxModel
    {
        [Required]
        public int ContactId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Reason { get; set; }
    }
}
