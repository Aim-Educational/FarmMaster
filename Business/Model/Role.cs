using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public int HierarchyOrder { get; set; } // Lower = Higher up in the hierarchy.

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

        [NotMapped]
        public bool IsGodRole => this.RoleId == 1;
    }

    public static class RoleExtentions
    {
        public static bool CanModify(this Role me, Role other)
        {
            return ((me?.HierarchyOrder ?? int.MaxValue) < (other?.HierarchyOrder ?? int.MaxValue))
                || me.IsGodRole;
        }
    }
}
