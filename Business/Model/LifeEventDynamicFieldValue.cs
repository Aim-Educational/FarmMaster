using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class LifeEventDynamicFieldValue
    {
        [Required]
        public int LifeEventDynamicFieldValueId { get; set; }

        [Required]
        public DynamicField Value { get; set; }

        [Required]
        public int LifeEventDynamicFieldInfoId { get; set; }
        public LifeEventDynamicFieldInfo LifeEventDynamicFieldInfo { get; set; }

        [Required]
        public int LifeEventEntryId { get; set; }
        public LifeEventEntry LifeEventEntry { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
