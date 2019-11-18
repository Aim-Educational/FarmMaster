using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class MapAnimalToAnimalGroup
    {
        [Required]
        public int MapAnimalToAnimalGroupId { get; set; }

        [Required]
        public int AnimalId { get; set; }
        public Animal Animal { get; set; }

        [Required]
        public int AnimalGroupId { get; set; }
        public AnimalGroup AnimalGroup { get; set; }
    }
}
