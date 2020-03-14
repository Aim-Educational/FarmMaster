using DataAccess.Constants;
using DataAccess.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class ApplicationUser : IdentityUser<int>
    {
    }

    public class ApplicationUserClaim : IdentityUserClaim<int>
    {
    }

    public class ApplicationUserLogin : IdentityUserLogin<int>
    {
    }

    public class ApplicationRole : IdentityRole<int>
    { 
    }

    public class ApplicationRoleClaim : IdentityRoleClaim<int>
    {
    }

    public class ApplicationUserRole : IdentityUserRole<int>
    { 
    }

    public class ApplicationUserToken : IdentityUserToken<int>
    {
    }

    public class IdentityContext : IdentityDbContext<
        ApplicationUser, 
        ApplicationRole, 
        int, 
        ApplicationUserClaim, 
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationRoleClaim, 
        ApplicationUserToken
    >
    {
        public IdentityContext(DbContextOptions<IdentityContext> context) : base(context)
        {
        }

        public void Seed(RoleManager<ApplicationRole> roles, UserManager<ApplicationUser> users)
        {
            this.SeedRoles(roles);
            this.SeedUsers(users);
        }

        private void SeedRoles(RoleManager<ApplicationRole> roles)
        {
            var roleInfo = new[]
            {
                new ApplicationRole { Name = RoleNames.SUPER_ADMIN }
            };

            foreach(var info in roleInfo)
            {
                info.NormalizedName = info.Name.ToUpper();
                if(!roles.RoleExistsAsync(info.Name).Result)
                    roles.CreateAsync(info).Wait();
            }
        }

        private void SeedUsers(UserManager<ApplicationUser> users)
        {
            var existingUser = users.FindByEmailAsync("admin@example.com").Result;
            if (existingUser != null)
                return;

            var user = new ApplicationUser
            {
                Email              = "admin@example.com",
                EmailConfirmed     = true,
                NormalizedEmail    = "ADMIN@EXAMPLE.COM",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                UserName           = "admin@example.com"
            };

            user.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(user, "password");

            // Create default superadmin
            users.CreateAsync(user).Wait();
            users.AddToRoleAsync(user, RoleNames.SUPER_ADMIN).Wait();
        }
    }

    public class IdentityContextFactory : FarmMasterContextFactory<IdentityContext>
    {
    }
}
