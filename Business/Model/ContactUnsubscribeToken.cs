using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    /// <summary>
    /// The normal contact tokens apply to the entire contact/user, not just a specific email.
    /// 
    /// Hence the need to create a special token just for this.
    /// 
    /// And since emails can be shared between contacts, unsub tokens can't be bound to singular contacts like a normal
    /// token can.
    /// </summary>
    public class ContactUnsubscribeToken
    {
        [Key]
        public int ContactUnsubscribeTokenId { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTimeOffset ExpiresUtc { get; set; }
    }
}
