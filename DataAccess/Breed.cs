using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccess
{
    public class Breed
    {
        public const int MAX_NAME_LENGTH = Species.MAX_NAME_LENGTH;

        [Key]
        public int BreedId { get; set; }

        [StringLength(MAX_NAME_LENGTH)]
        public string Name { get; set; }

        [Required]
        public int? SpeciesId { get; set; }
        public Species Species { get; set; }

        public int? NoteOwnerId { get; set; }
        public NoteOwner NoteOwner { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
