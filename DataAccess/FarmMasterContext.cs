using DataAccess.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
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
            b.Entity<Species>()
             .HasMany(s => s.Breeds)
             .WithOne(br => br.Species)
             .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Breed>()
             .HasOne(br => br.NoteOwner)
             .WithMany()
             .OnDelete(DeleteBehavior.Cascade);

            this.Seed(b);
        }

        #region Tables
        public DbSet<Settings>  Settings    { get; set; }
        public DbSet<LogEntry>  LogEntries  { get; set; }
        public DbSet<Contact>   Contacts    { get; set; }
        public DbSet<NoteOwner> NoteOwners  { get; set; }
        public DbSet<NoteEntry> NoteEntries { get; set; }
        public DbSet<Species>   Species     { get; set; }
        public DbSet<Breed>     Breeds      { get; set; }
        #endregion

        // Identity uses a runtime call to Seed because I need to make use of the UserManager etc. services to ensure
        // I don't mess things up.
        //
        // FarmMasterContext however will use the usual EF migration seeding, as we don't need to rely on services for this.
        #region Seeding
        void Seed(ModelBuilder b)
        {
            this.SeedSpeciesBreeds(b);
        }

        void SeedSpeciesBreeds(ModelBuilder b)
        {
            // So we don't have a chance in hell to overwrite user-made species/breeds.
            const int BASE_ID = int.MaxValue - 100_000;

            // Id will be added onto BASE_ID
            var speciesList = new List
            <
                (int id, string name, int gestrationDays, (int id, string name)[] breeds)
            >()
            {
                (1, "Cow", 283, new[]
                { 
                    (1,  "English Longhorn"),
                    (2,  "Red Poll"),
                    (3,  "White Park"),
                    (4,  "Hereford"),
                    (5,  "Highland"),
                    (6,  "Aryshire"),
                    (7,  "Aberdeen Angus"),
                    (8,  "South Devon"),
                    (9,  "British White"),
                    (10, "Belted Galloway")
                })
            };

            // Ensure we don't have duplicate ids
            var speciesIndexSet = new HashSet<int>();
            var breedIndexSet   = new HashSet<int>();
            foreach(var species in speciesList)
            {
                foreach(var breed in species.breeds)
                {
                    if(!breedIndexSet.Add(breed.id))
                        throw new InvalidOperationException($"Breed {breed.name} of Species {species.name} has duplicate ID of {breed.id}");
                }

                if(!speciesIndexSet.Add(species.id))
                    throw new InvalidOperationException($"Species {species.name} has duplicate ID of {species.id}");
            }

            // Seed all the data.
            b.Entity<Species>()
            .HasData(
                speciesList
                .Select(s => new Species { SpeciesId = s.id, Name = s.name, GestrationPeriod = TimeSpan.FromDays(s.gestrationDays) })
            );

            b.Entity<Breed>()
            .HasData(
                speciesList
                .SelectMany(s => 
                    s
                    .breeds
                    .Select(br => new Breed { BreedId = br.id, SpeciesId = s.id, Name = br.name })
                )
            );
        }
        #endregion
    }

    public class TrueFarmMasterContextFactory : FarmMasterContextFactory<FarmMasterContext>
    {
    }
}
