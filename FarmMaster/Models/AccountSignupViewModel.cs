using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;

namespace FarmMaster.Models
{
    public class AccountSignupLoginInfo
    {
        [Required]
        [StringLength(75)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(255)]
        public string PasswordConfirm { get; set; }
    }

    public class AccountSignupConsentInfo
    {
        [Required]
        public bool TermsOfServiceConsent { get; set; }

        [Required]
        public bool PrivacyPolicyConsent { get; set; }
    }

    public class AccountSignupViewModel
    {
        [Required]
        public AccountSignupLoginInfo LoginInfo { get; set; }

        [Required]
        public AccountSignupConsentInfo ConsentInfo { get; set; }
        
        [Required]
        public Contact Contact { get; set; }

        [Required]
        [MinLength(1)]
        public string[] TelephoneNumbers { get; set; }
    }
}
