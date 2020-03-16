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
        [StringLength(256)]
        public string Username { get; set; }

        [Required]
        [StringLength(256)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public bool ConfirmEmail { get; set; } // Set by query string to show "Please confirm your email"
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
}
