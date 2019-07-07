using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class Email
    {
        [Key]
        public int EmailId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Address { get; set; }

        [Required]
        public int ContactId { get; set; }
        public Contact Contact { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
