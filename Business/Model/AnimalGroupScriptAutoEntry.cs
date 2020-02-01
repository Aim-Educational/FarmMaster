using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class AnimalGroupScriptAutoEntry
    {
        [Key]
        public int AnimalGroupScriptAutoEntryId { get; set; }

        [Required]
        public int AnimalGroupId { get; set; }
        public AnimalGroup AnimalGroup { get; set; }

        [Required]
        public int AnimalGroupScriptId { get; set; }
        public AnimalGroupScript AnimalGroupScript { get; set; }

        [Required]
        public JObject Parameters { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
