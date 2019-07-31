using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;

namespace FarmMaster.Models
{
    public class SpeciesBreedIndexViewModel : ViewModelWithMessage
    {
        public IEnumerable<Species> Species { get; set; }
        public IEnumerable<Breed> Breeds { get; set; }
    }

    public class SpeciesCreateViewModel : ViewModelWithMessage
    {
        [Required]
        public string Name { get; set; }
        public bool IsPoultry { get; set; }
    }

    public class SpeciesEditViewModel : ViewModelWithMessage
    {
        [Required]
        public Species Species { get; set; }
    }
}
