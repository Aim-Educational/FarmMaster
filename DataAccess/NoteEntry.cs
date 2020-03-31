using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccess
{
    public class NoteEntry
    {
        public const int MAX_CATEGORY_LENGTH = 75;
        public const int MAX_CONTENT_LENGTH = 256;

        [Key]
        public int NoteEntryId { get; set; }

        [Required]
        [StringLength(MAX_CATEGORY_LENGTH)]
        public string Category { get; set; }

        [Required]
        [StringLength(MAX_CONTENT_LENGTH)]
        public string Content { get; set; }

        [Required]
        public int NoteOwnerId { get; set; }
        public NoteOwner NoteOwner { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
