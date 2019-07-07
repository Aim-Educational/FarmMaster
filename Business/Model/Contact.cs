using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        [Required]
        [StringLength(150)]
        public string FullName { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public IEnumerable<Telephone> PhoneNumbers { get; set; }
        public IEnumerable<Email> EmailAddresses{ get; set; }
    }
}
