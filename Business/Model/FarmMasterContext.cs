using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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

            b.Entity<Contact>()
             .HasIndex(c => c.Email)
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

            this.SeedRolePermissions(b);
        }

        public DbSet<User>                      Users                       { get; set; }
        public DbSet<Contact>                   Contacts                    { get; set; }
        public DbSet<Telephone>                 Telephones                  { get; set; }
        public DbSet<EnumRolePermission>        EnumRolePermissions         { get; set; }
        public DbSet<MapRolePermissionToRole>   MapRolePermissionToRoles    { get; set; }
        public DbSet<Role>                      Roles                       { get; set; }
        public DbSet<UserLoginInfo>             UserLoginInfo               { get; set; }
        public DbSet<UserPrivacy>               UserPrivacy                 { get; set; }
        #endregion

        #region Data seeding
        private void SeedRolePermissions(ModelBuilder b)
        {
            b.Entity<EnumRolePermission>()
             .HasData(
                new EnumRolePermission { EnumRolePermissionId = 1, InternalName = EnumRolePermissionNames.EDIT_CONTACTS, Description = "Edit Contacts" },
                new EnumRolePermission { EnumRolePermissionId = 2, InternalName = EnumRolePermissionNames.VIEW_CONTACTS, Description = "View Contacts" },
                new EnumRolePermission { EnumRolePermissionId = 3, InternalName = EnumRolePermissionNames.EDIT_ROLES, Description = "Edit Roles" },
                new EnumRolePermission { EnumRolePermissionId = 4, InternalName = EnumRolePermissionNames.VIEW_ROLES, Description = "View Roles" },
                new EnumRolePermission { EnumRolePermissionId = 5, InternalName = EnumRolePermissionNames.EDIT_USERS, Description = "Edit Users" },
                new EnumRolePermission { EnumRolePermissionId = 6, InternalName = EnumRolePermissionNames.VIEW_USERS, Description = "View Users" },
                new EnumRolePermission { EnumRolePermissionId = 7, InternalName = EnumRolePermissionNames.ASSIGN_ROLES, Description = "Assign Roles" }
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
