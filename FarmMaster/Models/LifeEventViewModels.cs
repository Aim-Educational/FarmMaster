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
        Edit,
        Test
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

        [Required]
        public LifeEvent.TargetType Target { get; set; }
    }

    public class LifeEventEditViewModel : LifeEventCreateViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public IDictionary<LifeEvent.TargetFlags, bool> Flags { get; set; }

        public IEnumerable<LifeEventDynamicFieldInfo> GET_Fields { get; set; }
        
        public bool GET_IsInUse { get; set; }

        [Required]
        public LifeEvent.TargetType GET_Target { get; set; }
    }

    public class LifeEventEntryEditorViewModel : ViewModelWithMessage
    {
        [Required]
        public new LifeEventEntryEditorType Type { get; set; }

        [Required]
        public int LifeEventId { get; set; }

        // Passed through by an external controller. Used so the redirect action can easily take the user back to a certain page.
        [Required]
        public int RedirectEntityId { get; set; } 

        public int LifeEventEntryId { get; set; } // Only used in Edit mode.

        public string LifeEventName { get; set; }
        
        [Required]
        public string RedirectController { get; set; }

        [Required]
        public string RedirectAction { get; set; }

        [Required]
        public IEnumerable<LifeEventDynamicFieldInfo> GET_FieldInfo;

        [Required]
        public Dictionary<string, string> Breadcrumb { get; set; }

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
}
