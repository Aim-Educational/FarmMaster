using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    /// <summary>
    /// A group of animals.
    /// </summary>
    public class AnimalGroup
    {
        [Required]
        public int AnimalGroupId { get; set; }

        [StringLength(75)]
        public string Name { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        public IEnumerable<MapAnimalToAnimalGroup> Animals { get; set; }
        public IEnumerable<AnimalGroupScriptAutoEntry> AutomatedScripts { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
