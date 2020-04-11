using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccess
{
    public class Species
    {
        public const int MAX_NAME_LENGTH = 75;

        [Key]
        public int SpeciesId { get; set; }

        [Required]
        [StringLength(MAX_NAME_LENGTH)]
        public string Name { get; set; }

        [Required]
        public TimeSpan GestrationPeriod { get; set; }

        public int? NoteOwnerId { get; set; }
        public NoteOwner NoteOwner { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
