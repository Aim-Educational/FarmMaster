using DataAccess.Constants;
using DataAccess.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Linq;

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
            var roleInfo = new(string name, string[] perms)[]
            {
                (Constants.Roles.SuperAdmin, Permissions.AllPermissions)
            };

            foreach(var info in roleInfo)
            {
                var role = new ApplicationRole
                {
                    Name = info.name,
                    NormalizedName = info.name.ToUpper()
                };

                if(!roles.RoleExistsAsync(role.Name).Result)
                    roles.CreateAsync(role).Wait();
                else
                    role = roles.FindByNameAsync(role.Name).Result;

                // Always try to add new permissions
                var roleClaims = roles.GetClaimsAsync(role).Result;
                foreach(var perm in info.perms)
                {
                    var claim = new Claim(Permissions.ClaimType, perm);

                    if(!roleClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                        roles.AddClaimAsync(role, claim).Wait();
                }
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
            users.AddToRoleAsync(user, Constants.Roles.SuperAdmin).Wait();
        }
    }

    public class IdentityContextFactory : FarmMasterContextFactory<IdentityContext>
    {
    }
}
