using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class Breed
    {
        [Key]
        public int BreedId { get; set; }

        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [Required]
        public bool IsRegisterable { get; set; }

        [Required]
        public int BreedSocietyId { get; set; }
        public Contact BreedSociety { get; set; }

        [Required]
        public int SpeciesId { get; set; }
        public Species SpeciesType { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
