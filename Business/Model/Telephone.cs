using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class Telephone
    {
        [Key]
        public int TelephoneId { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        [StringLength(15)]
        public string Number { get; set; }

        [Required]
        public int ContactId { get; set; }
        public Contact Contact { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
