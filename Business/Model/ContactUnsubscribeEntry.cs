using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    /// <summary>
    /// Created to detail that a certain email address should no longer be send information
    /// about a certain contact.
    /// </summary>
    public class ContactUnsubscribeEntry
    {
        [Key]
        public int ContactUnsubscribeEntryId { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
    }
}
