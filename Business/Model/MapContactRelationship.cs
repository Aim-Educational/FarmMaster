using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class MapContactRelationship
    {
        [Key]
        public int MapContactRelationshipId { get; set; }

        [Required]
        public int ContactOneId { get; set; }
        public Contact ContactOne { get; set; }

        [Required]
        public int ContactTwoId { get; set; }
        public Contact ContactTwo { get; set; }

        [Required]
        [StringLength(64)]
        public string Description { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public Contact GetContactNotMe(Contact me)
        {
            Contact notMe = null;
            if(this.ContactOneId == me.ContactId)
                notMe = this.ContactTwo;
            if(this.ContactTwoId == me.ContactId)
                notMe = this.ContactOne;

            if(notMe == null)
                throw new InvalidOperationException("Neither contact is you. (Or the contacts need to be loaded with .Include())");

            return notMe;
        }
    }
}
