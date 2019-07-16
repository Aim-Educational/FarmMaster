using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Misc;

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

    public class ContactCreateViewModel : ViewModelWithMessage
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public Contact.Type Type { get; set; }

        [Required]
        [RegularExpression(GlobalConstants.RegexEmail, ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }
    }

    public class ContactAjaxAddPhoneNumber : AjaxModel
    {
        [Required]
        public int ContactId { get; set; }

        [Required(ErrorMessage = "A name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A number is required.")]
        [RegularExpression(GlobalConstants.RegexPhone, ErrorMessage = "Invalid Phone Number. Only '+', and digits are allowed.")]
        public string Value { get; set; }

        [Required(ErrorMessage = "You must give a reason for adding this number.")]
        public string Reason { get; set; }
    }

    public class ContactAjaxAddEmailAddress : AjaxModel
    {
        [Required]
        public int ContactId { get; set; }

        [Required(ErrorMessage = "A name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "An email is required.")]
        [RegularExpression(GlobalConstants.RegexEmail, ErrorMessage = "Invalid Email Address.")]
        public string Value { get; set; }

        [Required(ErrorMessage = "You must give a reason for adding this number.")]
        public string Reason { get; set; }
    }

    public class ContactAjaxRemoveByName : AjaxModel
    {
        [Required]
        public int ContactId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Reason { get; set; }
    }
}
