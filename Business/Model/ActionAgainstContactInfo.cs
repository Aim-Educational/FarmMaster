using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class ActionAgainstContactInfo
    {
        public enum Type
        {
            Unknown,
            View_ContactInfo,
            Add_PhoneNumber,
            Delete_PhoneNumber
        }

        [Key]
        public int ActionAgainstContactInfoId { get; set; }

        [Required]
        public int UserResponsibleId { get; set; }
        public User UserResponsible { get; set; }

        [Required]
        public int ContactAffectedId { get; set; }
        public Contact ContactAffected { get; set; }

        [Required]
        public Type ActionType { get; set; }

        [Required]
        [StringLength(150)]
        public string Reason { get; set; }
        
        [StringLength(150)]
        public string AdditionalInfo { get; set; }

        [Required]
        public DateTimeOffset DateTimeUtc { get; set; }

        [Required]
        public bool HasContactBeenInformed { get; set; }
    }
}
