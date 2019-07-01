using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class UserLoginInfo
    {
        [Key]
        public int UserLoginInfoId { get; set; }

        [Required]
        [StringLength(75)]
        public string Username { get; set; } 

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(60)]
        public byte[] PassHash { get; set; }

        [Required]
        public string Salt { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
