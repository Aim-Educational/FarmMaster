using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class Animal
    {
        public enum Gender
        {
            Male,
            Female,
            Other
        }

        [Key]
        public int AnimalId { get; set; }

        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [Required]
        public Gender Sex { get; set; }

        [Required]
        [StringLength(20)]
        public string Tag { get; set; }

        [Required]
        public int OwnerId { get; set; }
        public Contact Owner { get; set; }

        [Required]
        public int CharacteristicsId { get; set; }
        public AnimalCharacteristicList Characteristics { get; set; }

        public int? MumId { get; set; }
        public Animal Mum { get; set; }
        
        public int? DadId { get; set; }
        public Animal Dad { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public IEnumerable<MapBreedToAnimal> Breeds { get; set; }

        public IEnumerable<MapLifeEventEntryToAnimal> LifeEventEntries { get; set; }

        [NotMapped] // Sometimes EF sets it to null, sometimes it sets it to an empty list when there's no elements, so we'll try to handle both.
        public IEnumerable<Animal> Children => (this.Children_DAD?.Count() ?? 0) == 0
                                               ? this.Children_MUM
                                               : this.Children_DAD;

        // Use 'Children'
        [InverseProperty(nameof(Mum))]
        public IEnumerable<Animal> Children_MUM { get; set; }

        // Use 'Children'
        [InverseProperty(nameof(Dad))]
        public IEnumerable<Animal> Children_DAD { get; set; }
    }
}
