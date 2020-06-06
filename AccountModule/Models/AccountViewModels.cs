using System.ComponentModel.DataAnnotations;

namespace AccountModule.Models
{
    public class AccountLoginViewModel
    {
        [Required]
        [StringLength(256)]
        public string Username { get; set; }

        [Required]
        [StringLength(256)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public bool ConfirmEmail { get; set; } // Set by query string to show "Please confirm your email"

        public string Error { get; set; } // ^^

        public bool Success { get; set; } // ^^
    }

    public class AccountRegisterViewModel
    {
        [Required]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [StringLength(256)]
        public string Username { get; set; }


        [Required]
        [StringLength(256)]
        public string Password { get; set; }

        [Required]
        [StringLength(256)]
        public string ConfirmPassword { get; set; }
    }

    public class AccountFinaliseExternalLoginViewModel
    {
        [Required]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [StringLength(256)]
        public string Username { get; set; }
    }

    public class AccountResendEmailViewModel
    {
        [Required]
        [StringLength(256)]
        public string Email { get; set; }
    }
}
