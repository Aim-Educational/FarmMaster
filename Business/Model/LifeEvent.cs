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
        /// <summary>
        /// Life events will eventually be extended onto things like vehicles, so 
        /// we need to specify what targets each life event applies to.
        /// </summary>
        public enum TargetType
        {
            Unknown,
            Animal
        }

        /// <summary>
        /// Flags relating to the life event, and special functions it may perform on its targets.
        /// 
        /// Where possible, keep flags ambiguous so I don't need to add functionality to filter
        /// them based on target type.
        /// </summary>
        [Flags]
        public enum TargetFlags
        {
            /// <summary>
            /// Failsafe value.
            /// </summary>
            None = 0,

            /// <summary>
            /// Mark that the target has reached end of system (sold, died, broke, etc.)
            /// </summary>
            EndOfSystem = 1 << 0,

            /// <summary>
            /// The life even is builtin, so cannot be modified by a user.
            /// </summary>
            IsBuiltin = 1 << 1
        }

        [Required]
        public int LifeEventId { get; set; }

        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        public TargetType Target { get; set; }

        [Required]
        public TargetFlags Flags { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public IEnumerable<LifeEventDynamicFieldInfo> Fields { get; set; }
        public IEnumerable<LifeEventEntry> Entries { get; set; }

        [NotMapped]
        public bool IsInUse => (this.Entries.Count() > 0);

        [NotMapped]
        public bool IsBuiltin => (this.Flags & TargetFlags.IsBuiltin) > 0;

        [NotMapped]
        public bool IsEndOfSystem
        {
            get => (this.Flags & TargetFlags.EndOfSystem) > 0;
            set
            {
                if(value)
                    this.Flags |= TargetFlags.EndOfSystem;
                else
                    this.Flags &= ~TargetFlags.EndOfSystem;
            }
        }
    }
}
