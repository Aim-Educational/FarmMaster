using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class MapLifeEventEntryToAnimal
    {
        [Key]
        public int MapLifeEventEntryToAnimalId { get; set; }

        [Required]
        public int AnimalId { get; set; }
        public Animal Animal { get; set; }

        [Required]
        public int LifeEventEntryId { get; set; }
        public LifeEventEntry LifeEventEntry { get; set; }
    }
}
