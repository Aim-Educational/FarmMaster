using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class MetricRequest
    {
        [Key]
        public int MetricRequestId { get; set; }

        [Required]
        public uint Count { get; set; }

        [Required]
        public DateTimeOffset DateTimeUtc { get; set; }
    }
}
