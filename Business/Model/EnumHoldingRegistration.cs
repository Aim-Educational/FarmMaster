using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class EnumHoldingRegistration
    {
        public static class Names
        {
            public const string COW             = "cow";
            public const string PIG             = "pig";
            public const string SHEEP_AND_GOAT  = "sheep_goats";
            public const string FISH            = "fish";
            public const string POULTRY         = "poultry";
        }

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
