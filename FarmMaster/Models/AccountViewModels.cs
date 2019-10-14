using FarmMaster.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class AccountLoginViewModel
    {
        [Required]
        [StringLength(75)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(255)]
        public string Password { get; set; }

        // HTTP-GET ONLY
        public bool VerifyEmail { get; set; }
    }

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

        [Required]
        public bool AgeConsent { get; set; }
    }

    public class AccountSignupNameInfo
    {
        [Required]
        [RegularExpression("^[a-zA-Z-]+$")]
        public string FirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z-\s]+$")]
        public string MiddleNames { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z-]+$")]
        public string LastName { get; set; }
    }

    public class AccountSignupViewModel : ViewModelWithMessage
    {
        [Required]
        public AccountSignupLoginInfo LoginInfo { get; set; }

        [Required]
        public AccountSignupConsentInfo ConsentInfo { get; set; }

        [Required]
        public AccountSignupNameInfo NameInfo { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(1)]
        public IList<string> TelephoneNumbers { get; set; }
    }

    public class AccountForgotPasswordViewModel : ViewModelWithMessage
    {
        [Required]
        [StringLength(75)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(FarmConstants.Regexes.Email)]
        public string Email { get; set; }
    }

    public class AccountResetPasswordViewModel : ViewModelWithMessage
    {
        [Required]
        [StringLength(75)]
        public string Token { get; set; }

        [Required]
        [StringLength(75)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(75)]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }

    public class AccountAjaxWithPasswordRequest : AjaxRequestModel
    {
        [Required]
        [StringLength(75)]
        public string Password { get; set; }
    }
}
