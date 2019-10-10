using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Business.Model
{
    /// <summary>
    /// NOTE: Some tokens might be specific for users.
    /// It's easier to just extend them for both contacts and users (via their contacts), and just check to make sure the 
    /// referenced contact is for a user for user-specific tokens.
    /// </summary>
    public class ContactToken
    {
        public enum Type
        {
            N_A,
            Anonymise, // For contacts: Anonymise the contact.
                       // For users: Anonymise the user *and* the contact.
        }

        [Key]
        public int ContactTokenId { get; set; }

        [Required]
        [StringLength(36)] // 36 = Length of Guid.NewGuid().ToString()
        public string Token { get; set; }

        [Required]
        public Type UsageType { get; set; }

        [Required]
        public int ContactId { get; set; }
        public Contact Contact { get; set; }

        [Required]
        public DateTimeOffset Expiry { get; set; }

        [NotMapped]
        public bool HasExpired => DateTimeOffset.UtcNow >= this.Expiry;
    }
}
