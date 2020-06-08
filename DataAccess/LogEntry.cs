using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess
{
    /// <summary>
    /// Contains information about something that has happened, such as an exception, or a user being added, etc.
    /// </summary>
    public class LogEntry
    {
        public const int MAX_TEXT_LENGTH = 512;
        public const int MAX_DATA_LENGTH = 2048;
        public const int MAX_GENERIC_LENGTH = 75;

        [Key]
        public int LogEntryId { get; set; }

        [Required]
        [StringLength(MAX_GENERIC_LENGTH)]
        public string CategoryName { get; set; }

        [Required]
        public int EventId { get; set; }

        [StringLength(MAX_GENERIC_LENGTH)]
        public string EventName { get; set; }

        [Required]
        public LogLevel Level { get; set; }

        [Required]
        [StringLength(MAX_TEXT_LENGTH)]
        public string Message { get; set; }

        [StringLength(MAX_DATA_LENGTH)]
        [Column(TypeName = "jsonb")]
        public string StateJson { get; set; }

        [Required]
        public DateTimeOffset DateLogged { get; set; }
    }
}
