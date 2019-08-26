using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public IEnumerable<LifeEventDynamicFieldInfo> Fields { get; set; }
    }
}
