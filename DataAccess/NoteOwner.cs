using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DataAccess
{
    /// <summary>
    /// e.g.
    /// 
    /// Contact has one NoteOwner.
    /// NoteOwners have many NoteEntries.
    /// 
    /// This is just here so we can uniformly group notes together.
    /// </summary>
    public class NoteOwner
    {
        [Key]
        public int NoteOwnerId { get; set; }

        public ICollection<NoteEntry> NoteEntries { get; set; }
    }
}
