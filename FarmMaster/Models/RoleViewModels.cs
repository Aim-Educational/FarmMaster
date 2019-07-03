using Business.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class RoleCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        public Dictionary<string, bool> Permissions { get; set; }

        // MUST BE REFRESHED EVERY REQUEST.
        public IEnumerable<EnumRolePermission> AllKnownPermissions { get; set; }
    }

    public class RoleEditViewModel
    {
        public Role Role { get; set; }
        public Dictionary<string, bool> Permissions { get; set; }

        // MUST BE REFRESHED EVERY REQUEST.
        public IEnumerable<EnumRolePermission> AllKnownPermissions { get; set; }
    }

    // GET only.
    public class RoleIndexViewModel : ViewModelWithMessage
    {
        public IEnumerable<Role> Roles;
    }
}
