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
}
