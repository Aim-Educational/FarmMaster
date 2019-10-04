using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
        
        public bool IsRegisterable { get; set; }

        [Required]
        public int BreedSocietyId { get; set; }
        public Contact BreedSociety { get; set; }

        [Required]
        public int SpeciesId { get; set; }
        public Species Species { get; set; }
        
        [Required]
        public int CharacteristicListId { get; set; }
        public AnimalCharacteristicList CharacteristicList { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public IEnumerable<MapBreedToAnimal> Mappings { get; set; }
        
        [NotMapped]
        public bool IsSafeToDelete
        {
            get
            {
                return this.Mappings.Count() == 0;
            }
        }
    }
}
