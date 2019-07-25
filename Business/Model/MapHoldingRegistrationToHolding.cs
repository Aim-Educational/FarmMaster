using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class MapHoldingRegistrationToHolding
    {
        [Key]
        public int MapHoldingRegistrationToHoldingId { get; set; }
        
        [Required]
        [StringLength(25)]
        public string HerdNumber { get; set; }

        [Required]
        public int HoldingId { get; set; }
        public Holding Holding { get; set; }

        [Required]
        public int HoldingRegistrationId { get; set; }
        public EnumHoldingRegistration HoldingRegistration { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
