using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Perms = Business.Model.BusinessConstants.Permissions;

namespace Business.Model
{
    public partial class FarmMasterContext : DbContext
    {
        #region Non-Model
        private string _connectionString;

        public FarmMasterContext(DbContextOptions options) : base(options)
        {
        }

        public FarmMasterContext(string connectionString)
        {
            this._connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(this._connectionString);
            }
        }
        #endregion

        #region Model
        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<User>()
             .HasIndex(u => u.ContactId)
             .IsUnique();

            b.Entity<MapRolePermissionToRole>()
             .HasIndex(m => new { m.EnumRolePermissionId, m.RoleId })
             .IsUnique();

            b.Entity<EnumRolePermission>()
             .HasIndex(r => r.InternalName)
             .IsUnique();

            b.Entity<Role>()
             .HasIndex(r => r.Name)
             .IsUnique();

            b.Entity<Contact>()
             .Property(c => c.ContactType)
             .HasConversion<string>();
            b.Entity<Contact>()
             .HasIndex(c => c.IsAnonymous);

            b.Entity<ActionAgainstContactInfo>()
             .Property(a => a.ActionType)
             .HasConversion<string>();
            b.Entity<ActionAgainstContactInfo>()
             .HasIndex(a => a.ContactAffectedId);
            b.Entity<ActionAgainstContactInfo>()
             .HasIndex(a => a.HasContactBeenInformed);

            b.Entity<MapContactRelationship>()
             .HasIndex(m => m.ContactOneId);
            b.Entity<MapContactRelationship>()
             .HasIndex(m => m.ContactTwoId);
            b.Entity<MapContactRelationship>()
             .HasIndex(nameof(MapContactRelationship.ContactOneId), nameof(MapContactRelationship.ContactTwoId))
             .IsUnique();

            b.Entity<MapHoldingRegistrationToHolding>()
             .HasIndex(m => m.HoldingId);
            b.Entity<MapHoldingRegistrationToHolding>()
             .HasIndex(nameof(MapHoldingRegistrationToHolding.HoldingId), nameof(MapHoldingRegistrationToHolding.HoldingRegistrationId))
             .IsUnique();

            b.Entity<Species>()
             .HasIndex(s => s.Name)
             .IsUnique();

            b.Entity<Breed>()
             .HasIndex(bb => bb.Name)
             .IsUnique();
            b.Entity<Breed>()
             .HasIndex(nameof(Breed.Name), nameof(Breed.SpeciesId))
             .IsUnique();

            b.Entity<AnimalCharacteristic>()
             .Property(c => c.Data)
             .IsDynamicField();

            b.Entity<LifeEventDynamicFieldInfo>()
             .Property(e => e.Type)
             .HasConversion<string>();
            b.Entity<LifeEventDynamicFieldInfo>()
             .HasIndex("Name", "LifeEventId")
             .IsUnique();

            b.Entity<LifeEventDynamicFieldValue>()
             .Property(v => v.Value)
             .IsDynamicField();
            b.Entity<LifeEventDynamicFieldValue>()
             .HasIndex(nameof(LifeEventDynamicFieldValue.LifeEventDynamicFieldInfoId), nameof(LifeEventDynamicFieldValue.LifeEventEntryId))
             .IsUnique();

            b.Entity<LifeEvent>()
             .HasIndex(e => e.Name)
             .IsUnique();

            b.Entity<LifeEventEntry>()
             .HasIndex(e => e.DateTimeCreated);
            b.Entity<LifeEventEntry>()
             .HasIndex(nameof(LifeEventEntry.DateTimeCreated), nameof(LifeEventEntry.LifeEventId));

            b.Entity<MapLifeEventEntryToAnimal>()
             .HasIndex(nameof(MapLifeEventEntryToAnimal.AnimalId), nameof(MapLifeEventEntryToAnimal.LifeEventEntryId))
             .IsUnique();
            b.Entity<MapLifeEventEntryToAnimal>()
             .HasIndex(m => m.LifeEventEntryId);
            b.Entity<MapLifeEventEntryToAnimal>()
             .HasIndex(m => m.AnimalId);

            b.Entity<MapBreedToAnimal>()
             .HasIndex(nameof(MapBreedToAnimal.AnimalId), nameof(MapBreedToAnimal.BreedId))
             .IsUnique();
            b.Entity<MapBreedToAnimal>()
             .HasIndex(m => m.AnimalId);
            b.Entity<MapBreedToAnimal>()
             .HasIndex(m => m.BreedId);

            b.Entity<ContactToken>()
             .HasIndex(t => t.UsageType);
            b.Entity<ContactToken>()
             .Property(t => t.UsageType)
             .HasConversion<string>();
            b.Entity<ContactToken>()
             .HasIndex(t => t.Token)
             .IsUnique();

            b.Entity<MapAnimalToAnimalGroup>()
             .HasIndex(nameof(MapAnimalToAnimalGroup.AnimalGroupId), nameof(MapAnimalToAnimalGroup.AnimalId))
             .IsUnique();

            this.SeedRolePermissions(b);
            this.SeedHoldingRegistrations(b);
            this.SeedLifeEvents(b);
        }

        public DbSet<User>                              Users                            { get; set; }
        public DbSet<Contact>                           Contacts                         { get; set; }
        public DbSet<Telephone>                         Telephones                       { get; set; }
        public DbSet<EnumRolePermission>                EnumRolePermissions              { get; set; }
        public DbSet<MapRolePermissionToRole>           MapRolePermissionToRoles         { get; set; }
        public DbSet<Role>                              Roles                            { get; set; }
        public DbSet<UserLoginInfo>                     UserLoginInfo                    { get; set; }
        public DbSet<UserPrivacy>                       UserPrivacy                      { get; set; }
        public DbSet<Email>                             Emails                           { get; set; }
        public DbSet<ActionAgainstContactInfo>          ActionsAgainstContactInfo        { get; set; }
        public DbSet<MapContactRelationship>            MapContactRelationships          { get; set; }
        public DbSet<EnumHoldingRegistration>           EnumHoldingRegistrations         { get; set; }
        public DbSet<Holding>                           Holdings                         { get; set; }
        public DbSet<MapHoldingRegistrationToHolding>   MapHoldingRegistrationToHoldings { get; set; }
        public DbSet<Species>                           Species                          { get; set; }
        public DbSet<Breed>                             Breeds                           { get; set; }
        public DbSet<AnimalCharacteristic>              AnimalCharacteristics            { get; set; }
        public DbSet<AnimalCharacteristicList>          AnimalCharacteristicLists        { get; set; }
        public DbSet<LifeEvent>                         LifeEvents                       { get; set; }
        public DbSet<LifeEventDynamicFieldInfo>         LifeEventDynamicFieldInfo        { get; set; }
        public DbSet<LifeEventDynamicFieldValue>        LifeEventDynamicFieldValues      { get; set; }
        public DbSet<LifeEventEntry>                    LifeEventEntries                 { get; set; }
        public DbSet<MapLifeEventEntryToAnimal>         MapLifeEventEntryToAnimals       { get; set; }
        public DbSet<MapBreedToAnimal>                  MapBreedToAnimals                { get; set; }
        public DbSet<Animal>                            Animals                          { get; set; }
        public DbSet<ContactToken>                      ContactTokens                    { get; set; }
        public DbSet<MetricRequest>                     MetricRequests                   { get; set; }
        public DbSet<Image>                             Images                           { get; set; }
        public DbSet<ImageData>                         ImageData                        { get; set; }
        public DbSet<AnimalGroup>                       AnimalGroups                     { get; set; }
        public DbSet<MapAnimalToAnimalGroup>            MapAnimalToAnimalGroups          { get; set; }
        #endregion

        #region Data seeding
        private void SeedRolePermissions(ModelBuilder b)
        {
            // (InternalName, Description)
            var roles = new[]
            {
                (Perms.ASSIGN_ROLES,            "Assign Roles"),
                (Perms.CREATE_ROLES,            "Create Roles"),
                (Perms.DELETE_ROLES,            "Delete Roles"),
                (Perms.EDIT_ROLES,              "Edit Roles"),
                (Perms.VIEW_ROLES,              "View Roles"),

                (Perms.CREATE_CONTACTS,         "Create Contacts"),
                (Perms.DELETE_CONTACTS,         "Delete Contacts"),
                (Perms.EDIT_CONTACTS,           "Edit Contacts"),
                (Perms.VIEW_CONTACTS,           "View Contacts"),

                (Perms.CREATE_HOLDINGS,         "Create Holdings"),
                (Perms.DELETE_HOLDINGS,         "Delete Holdings"),
                (Perms.EDIT_HOLDINGS,           "Edit Holdings"),
                (Perms.VIEW_HOLDINGS,           "View Holdings"),

                (Perms.CREATE_LIFE_EVENTS,      "Create Life Events"),
                (Perms.DELETE_LIFE_EVENTS,      "Delete Life Events"),
                (Perms.EDIT_LIFE_EVENTS,        "Edit Life Events"),
                (Perms.VIEW_LIFE_EVENTS,        "View Life Events"),
                (Perms.USE_LIFE_EVENT_ENTRY,    "Create Life Event Entries"),

                (Perms.CREATE_SPECIES_BREEDS,   "Create Species & Breeds"),
                (Perms.DELETE_SPECIES_BREEDS,   "Delete Species & Breeds"),
                (Perms.EDIT_SPECIES_BREEDS,     "Edit Species & Breeds"),
                (Perms.VIEW_SPECIES_BREEDS,     "View Species & Breeds"),

                (Perms.CREATE_ANIMALS,          "Create Animals"),
                (Perms.DELETE_ANIMALS,          "Delete Animals"),
                (Perms.EDIT_ANIMALS,            "Edit Animals"),
                (Perms.VIEW_ANIMALS,            "View Animals"),

                (Perms.CREATE_ANIMAL_GROUPS,    "Create Animal Groups"),
                (Perms.DELETE_ANIMAL_GROUPS,    "Delete Animal Groups"),
                (Perms.EDIT_ANIMAL_GROUPS,      "Edit Animal Groups"),
                (Perms.VIEW_ANIMAL_GROUPS,      "View Animal Groups")
            };

            // Try to keep things in the same order please, future me...
            var index = 1;
            b.Entity<EnumRolePermission>()
             .HasData(roles.Select(tuple => new EnumRolePermission
                      { 
                        EnumRolePermissionId = index++, 
                        InternalName         = tuple.Item1, 
                        Description          = tuple.Item2
                      })
             );
        }

        private void SeedHoldingRegistrations(ModelBuilder b)
        {
            // (InternalName, Description)
            var list = new[] 
            {
                (BusinessConstants.HoldingRegistrations.COW,            "Cows"),
                (BusinessConstants.HoldingRegistrations.FISH,           "Fish"),
                (BusinessConstants.HoldingRegistrations.PIG,            "Pigs"),
                (BusinessConstants.HoldingRegistrations.POULTRY,        "Poultry"),
                (BusinessConstants.HoldingRegistrations.SHEEP_AND_GOAT, "Sheep and Goats")
            };

            var index = 1;
            b.Entity<EnumHoldingRegistration>()
             .HasData(list.Select(tuple => new EnumHoldingRegistration
                      {
                        EnumHoldingRegistrationId = index++,
                        Description               = tuple.Item2,
                        InternalName              = tuple.Item1
                      })
             );
        }

        private void SeedSpecies(ModelBuilder b)
        {
        }

        private void SeedBreeds(ModelBuilder b)
        {
        }

        private void SeedLifeEvents(ModelBuilder b)
        {
            b.Entity<LifeEvent>()
             .HasData(
                new LifeEvent
                { 
                    LifeEventId = 1,
                    Name        = BusinessConstants.BuiltinLifeEvents.BORN,
                    Target      = LifeEvent.TargetType.Animal,
                    Description = "The animal was born.", 
                    Flags       = LifeEvent.TargetFlags.IsBuiltin | LifeEvent.TargetFlags.IsUnique
                },

                new LifeEvent
                {
                    LifeEventId = 1000,
                    Name        = BusinessConstants.BuiltinLifeEvents.ARCHIVED,
                    Target      = LifeEvent.TargetType.Animal,
                    Description = "The animal was archived by a user.",
                    Flags       = LifeEvent.TargetFlags.IsBuiltin | LifeEvent.TargetFlags.EndOfSystem | LifeEvent.TargetFlags.IsUnique
                }
            );
            
            b.Entity<LifeEventDynamicFieldInfo>()
             .HasData(
                #region BORN
                new LifeEventDynamicFieldInfo
                {
                    LifeEventDynamicFieldInfoId = 1,
                    LifeEventId                 = 1,
                    Name                        = BusinessConstants.BuiltinLifeEventFields.BORN_DATE,
                    Type                        = DynamicField.Type.DateTime,
                    Description                 = "When the animal was born." }
                #endregion
            );
        }
        #endregion
    }

    public class FarmMasterContextFactory : IDesignTimeDbContextFactory<FarmMasterContext>
    {
        const string APPSETTINGS = "../FarmMaster/appsettings.development.json";

        public FarmMasterContext CreateDbContext(string[] args)
        {
            if(File.Exists(APPSETTINGS))
            {
                var json = JObject.Parse(File.ReadAllText(APPSETTINGS));
                return new FarmMasterContext(json["ConnectionStrings"].Value<string>("Migrate"));
            }

            Console.Write($"Could not find {Path.GetFullPath(APPSETTINGS)} so please enter connection string manually.");
            Console.Write("Please enter a connection string for migration: ");
            return new FarmMasterContext(Console.ReadLine());
        }
    }
}
