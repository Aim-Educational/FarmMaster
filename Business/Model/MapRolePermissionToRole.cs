using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class MapRolePermissionToRole
    {
        [Key]
        public int MapRolePermissionToRoleId { get; set; }

        [Required]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        [Required]
        public int EnumRolePermissionId { get; set; }
        public EnumRolePermission EnumRolePermission { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
