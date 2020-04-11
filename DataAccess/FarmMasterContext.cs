using DataAccess.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class FarmMasterContext : DbContext
    {
        public FarmMasterContext(DbContextOptions options) : base(options)
        {
        }

        public static FarmMasterContext InMemory(string name = "test")
        {
            var options = new DbContextOptionsBuilder<FarmMasterContext>()
                .UseInMemoryDatabase(name)
                .Options;

            return new FarmMasterContext(options);
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<NoteOwner>()
             .HasMany(o => o.NoteEntries)
             .WithOne(e => e.NoteOwner);

            b.Entity<Contact>()
             .HasOne(c => c.NoteOwner);

            b.Entity<Species>()
             .HasOne(s => s.NoteOwner);
        }

        #region Tables
        public DbSet<Settings>  Settings    { get; set; }
        public DbSet<LogEntry>  LogEntries  { get; set; }
        public DbSet<Contact>   Contacts    { get; set; }
        public DbSet<NoteOwner> NoteOwners  { get; set; }
        public DbSet<NoteEntry> NoteEntries { get; set; }
        public DbSet<Species>   Species     { get; set; }
        #endregion
    }

    public class TrueFarmMasterContextFactory : FarmMasterContextFactory<FarmMasterContext>
    {
    }
}
