using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Business.Model
{
    public class AnimalCharacteristic
    {
        [Key]
        public int AnimalCharacteristicId { get; set; }

        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [Required]
        [StringLength(ushort.MaxValue)]
        public DynamicField Data { get; set; }

        [Required]
        public int ListId { get; set; }
        public AnimalCharacteristicList List { get; set; }
    }
}
