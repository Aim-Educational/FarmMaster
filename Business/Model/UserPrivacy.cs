using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class UserPrivacy
    {
        [Key]
        public int UserPrivacyId { get; set; }

        [Required]
        public bool HasVerifiedEmail { get; set; }

        [Required]
        public int TermsOfServiceVersionAgreedTo { get; set; }

        [Required]
        public int PrivacyPolicyVersionAgreedTo { get; set; }

        [Timestamp]
        byte[] Timestamp { get; set; }
    }
}
