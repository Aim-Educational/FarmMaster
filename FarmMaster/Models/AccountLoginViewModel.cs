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
}
