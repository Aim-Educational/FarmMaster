using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Business.Model
{
    public class Species
    {
        [Required]
        public int SpeciesId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public bool IsPoultry { get; set; }

        [Required]
        public int CharacteristicListId { get; set; }
        public AnimalCharacteristicList CharacteristicList { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public IEnumerable<Breed> Breeds { get; set; }
    }
}
