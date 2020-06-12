using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccess
{
    public class LocationHolding
    {
        const int MAX_STRING_LENGTH = 150;

        [Key]
        public int LocationHoldingId { get; set; }

        [Required]
        public int LocationId { get; set; }
        public Location Location { get; set; }

        [Required]
        [StringLength(MAX_STRING_LENGTH)]
        public string HoldingNumber { get; set; }

        [Required]
        [StringLength(MAX_STRING_LENGTH)]
        public string GridReference { get; set; }

        [Required]
        [StringLength(MAX_STRING_LENGTH)]
        public string Address { get; set; }

        [Required]
        [StringLength(MAX_STRING_LENGTH)]
        public string Postcode { get; set; }

        [Required]
        public int OwnerId { get; set; }
        public Contact Owner { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
