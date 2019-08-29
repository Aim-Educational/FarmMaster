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

        [Required]
        public IEnumerable<LifeEventDynamicFieldInfo> GET_Fields { get; set; }
        
        [Required]
        public bool GET_IsInUse { get; set; }
    }

    public class AjaxLifeEventAddFieldRequest : AjaxRequestModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DynamicField.Type Type { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int LifeEventId { get; set; }
    }

    public class AjaxLifeEventDeleteFieldRequest : AjaxRequestModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int LifeEventId { get; set; }
    }
}
