using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class LifeEventEntry
    {
        [Required]
        public int LifeEventEntryId { get; set; }

        [Required]
        public int LifeEventId { get; set; }
        public LifeEvent LifeEvent { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public IEnumerable<LifeEventDynamicFieldValue> Values { get; set; }
    }
}
