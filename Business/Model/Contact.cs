using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

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

        [Required]
        public bool IsAnonymous { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public IEnumerable<Telephone> PhoneNumbers { get; set; }
        public IEnumerable<Email> EmailAddresses{ get; set; }

        // Relationships have two references to Contact, and I don't trust/know how to make EF handle this properly on its own.
        public IQueryable<MapContactRelationship> GetRelationships(FarmMasterContext context)
        {
            return context.MapContactRelationships
                          .Include(r => r.ContactOne)
                          .Include(r => r.ContactTwo)
                          .Where(m => m.ContactOneId == this.ContactId || m.ContactTwoId == this.ContactId);
        }

        // Not too useful for entities.
        [NotMapped]
        public string FirstName => this.FullName.Split(' ').First();

        // Not too useful for entities.
        [NotMapped]
        public string LastName => this.FullName.Split(' ').Last();

        [NotMapped]
        public string ShortName => (this.LastName.Trim().Count() == 0)
                                   ? this.FirstName
                                   : $"{this.FirstName} {this.LastName}";

        [NotMapped]
        public string FirstNameWithAbbreviatedLastName =>
            this.FirstName + " " + this.LastName[0];
    }
}
