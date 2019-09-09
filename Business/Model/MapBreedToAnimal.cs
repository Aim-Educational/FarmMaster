using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class MapBreedToAnimal
    {
        [Key]
        public int MapBreedToAnimalId { get; set; }

        [Required]
        public int BreedId { get; set; }
        public Breed Breed { get; set; }

        [Required]
        public int AnimalId { get; set; }
        public Animal Animal { get; set; }
    }
}
