using Business.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class AnimalIndexViewModel : ViewModelWithMessage
    {
        // This is just a temp index model until I bother with the full model.
        [Required]
        public IEnumerable<string> AnimalNames { get; set; }
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
        public IEnumerable<int> BreedIds { get; set; } // Species can be calculated. Just make sure to do sanity checks.

        [Required]
        public Animal.Gender Sex { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public int? AnimalId { get; set; } // Should only be null for creation.

        public int? MumId { get; set; }

        public int? DadId { get; set; }

        public bool IsCreate { get; set; }
    }
}
