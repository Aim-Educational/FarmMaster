using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class LifeEvent
    { 
        [Required]
        public int LifeEventId { get; set; }

        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        public bool IsBuiltin { get; set; } // Builtin events can't be deleted or edited, as they may serve a special purpose internally.

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public IEnumerable<LifeEventDynamicFieldInfo> Fields { get; set; }
        public IEnumerable<LifeEventEntry> Entries { get; set; }

        [NotMapped]
        public bool IsInUse
        {
            get
            {
                return this.Entries.Count() > 0;
            }
        }
    }
}
