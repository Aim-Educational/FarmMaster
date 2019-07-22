using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class Holding
    {
        [Key]
        public int HoldingId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string HoldingNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string GridReference { get; set; }

        [Required]
        [StringLength(150)]
        public string Address { get; set; }

        [Required]
        [StringLength(15)]
        public string Postcode { get; set; }

        [Required]
        public int OwnerContactId { get; set; }
        public Contact OwnerContact { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public IEnumerable<MapHoldingRegistrationToHolding> Registrations { get; set; }

        public bool IsRegisteredFor(string raceInternalName)
        {
            return this.Registrations.Any(r => r.HoldingRegistration.InternalName == raceInternalName);
        }
    }
}
