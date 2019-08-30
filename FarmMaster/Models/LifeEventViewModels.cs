using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;

namespace FarmMaster.Models
{
    public enum LifeEventEntryEditorType
    {
        Create,
        Edit
    }

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

    public class LifeEventEntryEditorViewModel : ViewModelWithMessage
    {
        [Required]
        public new LifeEventEntryEditorType Type { get; set; }

        [Required]
        public int LifeEventId { get; set; }
        
        [Required]
        public string RedirectController { get; set; }

        [Required]
        public string RedirectAction { get; set; }

        [Required]
        public IEnumerable<LifeEventDynamicFieldInfo> GET_FieldInfo;

        /// <summary>
        /// Key is name of field, value an HTML string for the DynamicField.
        /// </summary>
        [Required]
        public IDictionary<string, string> Values { get; set; }
    }

    public class LifeEventEntryInputPartialViewModel
    {
        public LifeEventDynamicFieldInfo Info { get; set; }
        public IDictionary<string, string> Values { get; set; }
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
