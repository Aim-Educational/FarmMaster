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
        [StringLength(25)]
        public string TraceIdentifier { get; set; }

        [Required]
        [StringLength(100)]
        public string Path { get; set; }

        [Required]
        [StringLength(45)] // Max length of an IPv6 address
        public string Ip { get; set; }

        [Required]
        public long BytesUsedAtStart { get; set; }

        [Required]
        public long BytesUsedAtEnd { get; set; }

        [Required]
        public DateTimeOffset DateTimeUtc { get; set; }
    }
}
