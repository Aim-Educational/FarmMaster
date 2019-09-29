using Business.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class RoleCreateViewModel : ViewModelWithMessage
    {
        /// <summary>
        /// Only for edit mode.
        /// </summary>
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int HierarchyOrder { get; set; }

        public bool IsCreate { get; set; }

        public Dictionary<string, bool> Permissions { get; set; }

        public IEnumerable<EnumRolePermission> AllKnownPermissions { get; set; }
    }

    // GET only.
    public class RoleIndexViewModel : ViewModelWithMessage
    {
        public IEnumerable<Role> Roles;
    }

    // GET only. POST is done via AJAX.
    public class RoleAssignViewModel
    {
        public IEnumerable<User> Users;
        public IEnumerable<Role> Roles;
    }
}
