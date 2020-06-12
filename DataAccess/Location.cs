using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccess
{
    public enum LocationType
    {
        Unknown,
        Holding
    }

    public class Location
    {
        public const int MAX_NAME_LENGTH = 150;
        
        [Key]
        public int LocationId { get; set; }

        [Required]
        [StringLength(MAX_NAME_LENGTH)]
        public string Name { get; set; }

        [Required]
        public LocationType Type { get; set; }

        public int? HoldingId { get; set; }
        public LocationHolding Holding { get; set; }

        public int? NoteOwnerId { get; set; }
        public NoteOwner NoteOwner { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
