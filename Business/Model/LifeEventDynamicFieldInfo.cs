using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class LifeEventDynamicFieldInfo
    {
        public static class BuiltinNames
        {
            public const string BORN_DATE = "Date";
        }

        [Required]
        public int LifeEventDynamicFieldInfoId { get; set; }

        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        [Required]
        public DynamicField.Type Type { get; set; }

        [Required]
        public int LifeEventId { get; set; }
        public LifeEvent LifeEvent { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
