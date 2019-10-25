using Business.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class AnimalIndexViewModel : ViewModelWithMessage
    {
    }

    public class AnimalCreateEditViewModel : ViewModelWithMessage
    {
        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Tag { get; set; }

        [Required]
        public int? SpeciesId { get; set; }

        public IEnumerable<int> BreedIds { get; set; }

        [Required]
        public Animal.Gender Sex { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public IFormFile Image { get; set; }
        public int? ImageId { get; set; } // Only for edit mode. Only used for showing their image.

        public int? AnimalId { get; set; } // Should only be null for creation.

        public int? MumId { get; set; }

        public int? DadId { get; set; }

        public bool IsCreate { get; set; }
    }
}
