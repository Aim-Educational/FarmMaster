using DataAccess.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

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
             .HasOne(c => c.NoteOwner)
             .WithMany();

            b.Entity<Species>()
             .HasOne(s => s.NoteOwner)
             .WithMany();
            b.Entity<Species>()
             .HasMany(s => s.Breeds)
             .WithOne(br => br.Species)
             .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Breed>()
             .HasOne(br => br.NoteOwner)
             .WithMany();

            b.Entity<Location>()
             .HasOne(l => l.NoteOwner)
             .WithMany();

            b.Entity<LocationHolding>()
              .HasOne(h => h.Location)
              .WithOne(l => l.Holding)
              .HasForeignKey<LocationHolding>(h => h.LocationId)
              .OnDelete(DeleteBehavior.Cascade);
            b.Entity<LocationHolding>()
             .HasOne(h => h.Owner)
             .WithMany()
             .OnDelete(DeleteBehavior.Restrict);

            this.Seed(b);
        }

        #region Tables
        public DbSet<Settings> Settings { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<NoteOwner> NoteOwners { get; set; }
        public DbSet<NoteEntry> NoteEntries { get; set; }
        public DbSet<Species> Species { get; set; }
        public DbSet<Breed> Breeds { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationHolding> LocationHoldingInfo { get; set; }
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
            // So we have no chance in hell of overwriting user-made species/breeds.
            const int BASE_ID = int.MaxValue - 100_000;

            Func<int, string, int, ICollection<Breed>, Species> makeSpecies = (id, name, gestrationDays, breeds) =>
            {
                // For some very, very strange reason, if we don't use ID 1 for Species we get a weird error from ef-add-migration.
                if (id != 1)
                    id += BASE_ID;

                foreach (var breed in breeds)
                    breed.SpeciesId = id;

                return new Species
                {
                    SpeciesId = id,
                    Name = name,
                    GestrationPeriod = TimeSpan.FromDays(gestrationDays),
                    Breeds = breeds
                };
            };

            Func<int, string, Breed> makeBreed = (id, name) =>
            {
                id += BASE_ID;
                return new Breed
                {
                    BreedId = id,
                    Name = name
                };
            };

            var speciesList = new List<Species>()
            {
                makeSpecies(1, "Cow", 283, new List<Breed>
                {
                    makeBreed(1,  "English Longhorn"),
                    makeBreed(2,  "Red Poll"),
                    makeBreed(3,  "White Park"),
                    makeBreed(4,  "Hereford"),
                    makeBreed(5,  "Highland"),
                    makeBreed(6,  "Aryshire"),
                    makeBreed(7,  "Aberdeen Angus"),
                    makeBreed(8,  "South Devon"),
                    makeBreed(9,  "British White"),
                    makeBreed(10, "Belted Galloway")
                })
            };

            // Ensure we don't have duplicate ids
            var speciesIndexSet = new HashSet<int>();
            var breedIndexSet = new HashSet<int>();
            foreach (var species in speciesList)
            {
                foreach (var breed in species.Breeds)
                {
                    if (!breedIndexSet.Add(breed.BreedId))
                        throw new InvalidOperationException($"Breed {breed.Name} of Species {species.Name} has duplicate ID of {breed.BreedId}");
                }

                if (!speciesIndexSet.Add(species.SpeciesId))
                    throw new InvalidOperationException($"Species {species.Name} has duplicate ID of {species.SpeciesId}");
            }

            // Seed all the data.
            foreach (var species in speciesList)
            {
                // EF doesn't like us using navigation collections when seeding, so we need to set it to null first.
                var breeds = species.Breeds;
                species.Breeds = null;

                b.Entity<Species>().HasData(species);
                b.Entity<Breed>().HasData(breeds);
            }
        }
        #endregion
    }

    public class TrueFarmMasterContextFactory : FarmMasterContextFactory<FarmMasterContext>
    {
    }
}
