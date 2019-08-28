using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;

namespace FarmMaster.Models
{
    public class LifeEventIndexViewModel : ViewModelWithMessage
    {
        public IEnumerable<LifeEvent> LifeEvents { get; set; }
    }

    public class LifeEventCreateViewModel : ViewModelWithMessage
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }

    public class LifeEventEditViewModel : LifeEventCreateViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
