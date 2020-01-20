using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    /// <summary>
    /// Don't even ask how I got to this point.
    /// I don't know either. Run from the madness while you can.
    /// </summary>
    public class AnimalGroupScript
    {
        public const int MAX_BYTECODE_LENGTH = 1024 * 8;

        [Key]
        public int AnimalGroupScriptId { get; set; }

        [StringLength(75)]
        public string Name { get; set; }

        /// <summary>
        /// Don't say I didn't warn you.
        /// </summary>
        [StringLength(MAX_BYTECODE_LENGTH)]
        public string Code { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
