﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json.Linq;

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
        #endregion

        #region Data seeding
        private void SeedRolePermissions(ModelBuilder b)
        {
            b.Entity<EnumRolePermission>()
             .HasData(
                new EnumRolePermission { EnumRolePermissionId = 1,  InternalName = BusinessConstants.Roles.EDIT_CONTACTS,          Description = "Edit Contacts" },
                new EnumRolePermission { EnumRolePermissionId = 2,  InternalName = BusinessConstants.Roles.VIEW_CONTACTS,          Description = "View Contacts" },
                new EnumRolePermission { EnumRolePermissionId = 3,  InternalName = BusinessConstants.Roles.EDIT_ROLES,             Description = "Edit Roles" },
                new EnumRolePermission { EnumRolePermissionId = 4,  InternalName = BusinessConstants.Roles.VIEW_ROLES,             Description = "View Roles" },
                new EnumRolePermission { EnumRolePermissionId = 5,  InternalName = BusinessConstants.Roles.EDIT_USERS,             Description = "Edit Users" },
                new EnumRolePermission { EnumRolePermissionId = 6,  InternalName = BusinessConstants.Roles.VIEW_USERS,             Description = "View Users" },
                new EnumRolePermission { EnumRolePermissionId = 7,  InternalName = BusinessConstants.Roles.ASSIGN_ROLES,           Description = "Assign Roles" },
                new EnumRolePermission { EnumRolePermissionId = 8,  InternalName = BusinessConstants.Roles.DELETE_CONTACTS,        Description = "Delete Contacts" },
                new EnumRolePermission { EnumRolePermissionId = 9,  InternalName = BusinessConstants.Roles.VIEW_HOLDINGS,          Description = "View Holdings" },
                new EnumRolePermission { EnumRolePermissionId = 10, InternalName = BusinessConstants.Roles.EDIT_HOLDINGS,          Description = "Edit Holdings" },
                new EnumRolePermission { EnumRolePermissionId = 11, InternalName = BusinessConstants.Roles.VIEW_SPECIES_BREEDS,    Description = "View Species/Breeds" },
                new EnumRolePermission { EnumRolePermissionId = 12, InternalName = BusinessConstants.Roles.EDIT_SPECIES_BREEDS,    Description = "Edit Species/Breeds" },
                new EnumRolePermission { EnumRolePermissionId = 13, InternalName = BusinessConstants.Roles.VIEW_LIFE_EVENTS,       Description = "View Life Events" },
                new EnumRolePermission { EnumRolePermissionId = 14, InternalName = BusinessConstants.Roles.EDIT_LIFE_EVENTS,       Description = "Edit Life Events" },
                new EnumRolePermission { EnumRolePermissionId = 15, InternalName = BusinessConstants.Roles.EDIT_LIFE_EVENT_ENTRY,  Description = "Edit Life Event Entries" }
            );
        }

        private void SeedHoldingRegistrations(ModelBuilder b)
        {
            b.Entity<EnumHoldingRegistration>()
             .HasData(
                new EnumHoldingRegistration { EnumHoldingRegistrationId = 1, InternalName = BusinessConstants.HoldingRegistrations.COW,              Description = "Cows" },
                new EnumHoldingRegistration { EnumHoldingRegistrationId = 2, InternalName = BusinessConstants.HoldingRegistrations.FISH,             Description = "Fish" },
                new EnumHoldingRegistration { EnumHoldingRegistrationId = 3, InternalName = BusinessConstants.HoldingRegistrations.PIG,              Description = "Pigs" },
                new EnumHoldingRegistration { EnumHoldingRegistrationId = 4, InternalName = BusinessConstants.HoldingRegistrations.POULTRY,          Description = "Poultry" },
                new EnumHoldingRegistration { EnumHoldingRegistrationId = 5, InternalName = BusinessConstants.HoldingRegistrations.SHEEP_AND_GOAT,   Description = "Sheep and Goats" }
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
                new LifeEvent{ LifeEventId = 1, Name = BusinessConstants.BuiltinLifeEvents.BORN, Description = "The animal was born.", IsBuiltin = true }
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
        public FarmMasterContext CreateDbContext(string[] args)
        {
            Console.Write("Please enter a connection string for migration: ");
            return new FarmMasterContext(Console.ReadLine());
        }
    }
}
