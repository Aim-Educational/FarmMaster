using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class EnumRolePermission
    {
        [Key]
        public int EnumRolePermissionId { get; set; }

        [Required]
        [StringLength(50)]
        public string InternalName { get; set; }

        [Required]
        [StringLength(75)]
        public string Description { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }

    public static class EnumRolePermissionNames
    {
        public const string VIEW_CONTACTS = "view_contacts";
        public const string EDIT_CONTACTS = "edit_contacts";
        public const string VIEW_ROLES    = "view_roles";
        public const string EDIT_ROLES    = "edit_roles";
        public const string VIEW_USERS    = "view_users";
        public const string EDIT_USERS    = "edit_users";
    }
}
