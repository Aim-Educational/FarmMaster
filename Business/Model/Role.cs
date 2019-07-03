using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(150)]
        public string Description { get; set; }
        
        public int? ParentRoleId { get; set; }
        public Role ParentRole { get; set; }

        public IEnumerable<MapRolePermissionToRole> Permissions { get; set; }
        public IEnumerable<User> Users { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
