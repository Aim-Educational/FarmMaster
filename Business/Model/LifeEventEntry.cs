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

        [Required]
        public DateTimeOffset DateTimeCreated { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public IEnumerable<LifeEventDynamicFieldValue> Values { get; set; }

        // Don't auto include these, as their purpose is almost primarily for deleting data properly.
        #region External Refs
        public IEnumerable<MapLifeEventEntryToAnimal> AnimalMap { get; set; }
        #endregion
    }
}
