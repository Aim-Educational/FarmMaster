using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public string EmailVerificationToken { get; set; }

        [InverseProperty(nameof(Business.Model.User.UserPrivacy))]
        public User User { get; set; }

        [Timestamp]
        byte[] Timestamp { get; set; }
    }
}
