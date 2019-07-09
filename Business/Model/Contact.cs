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
        public enum Type
        {
            N_A,
            User,
            Individual,
            Entity // Business, Company, Charity, etc.
        }

        [Key]
        public int ContactId { get; set; }

        [Required]
        [StringLength(150)]
        public string FullName { get; set; }

        [Required]
        public Type ContactType { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public IEnumerable<Telephone> PhoneNumbers { get; set; }
        public IEnumerable<Email> EmailAddresses{ get; set; }

        // Not too useful for entities.
        [NotMapped]
        public string FirstName => this.FullName.Split(' ').First();

        // Not too useful for entities.
        [NotMapped]
        public string LastName => this.FullName.Split(' ').Last();
    }
}
