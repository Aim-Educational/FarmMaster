using DataAccess;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserModule.Models
{
    public class AdminUsersViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }
    }

    public class AdminManageUserViewModel
    {
        [Required]
        [StringLength(20)]
        public string Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Username { get; set; }

        [StringLength(256)]
        public string CurrentPassword { get; set; }

        [StringLength(256)]
        public string Password { get; set; }

        [StringLength(256)]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; } // Readonly, so we don't care about attributes

        public bool ShowDeleteButton { get; set; }
    }
}
