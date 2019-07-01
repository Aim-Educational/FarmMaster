using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public int ContactId { get; set; }
        public Contact Contact { get; set; }

        public int? RoleId { get; set; }
        public Role Role { get; set; }

        [Required]
        public int UserLoginInfoId { get; set; }
        public UserLoginInfo UserLoginInfo { get; set; }

        [Required]
        public int UserPrivacyId { get; set; }
        public UserPrivacy UserPrivacy { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
