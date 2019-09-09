using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class EnumHoldingRegistration
    {
        [Key]
        public int EnumHoldingRegistrationId { get; set; }

        [Required]
        [StringLength(50)]
        public string InternalName { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }
    }
}
