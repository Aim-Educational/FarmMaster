using Business.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class AnimalCreateViewModel : ViewModelWithMessage
    {
        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Tag { get; set; }

        [Required]
        public Animal.Gender Sex { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public int? MumId { get; set; }

        public int? DadId { get; set; }
    }
}
