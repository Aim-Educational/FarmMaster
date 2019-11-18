using Business.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class AnimalGroupIndexViewModel
    {
        public IEnumerable<AnimalGroup> Groups { get; set; }
    }

    public class AnimalGroupCreateEditViewModel : ViewModelWithMessage
    {
        public bool IsCreate { get; set; }

        // Common
        [Required]
        public int GroupId { get; set; }

        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [Required]
        [StringLength(150)]
        public string Description { get; set; }
    } 
}
