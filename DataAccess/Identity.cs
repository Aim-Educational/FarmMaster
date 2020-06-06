using DataAccess.Constants;
using DataAccess.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

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
        public const string DEFAULT_USERNAME = "admin@example.com";
        public const string DEFAULT_PASSWORD = "password";

        public IdentityContext(DbContextOptions<IdentityContext> context) : base(context)
        {
        }

        public void Seed(RoleManager<ApplicationRole> roles, UserManager<ApplicationUser> users)
        {
            this.SeedRoles(roles);
            this.SeedUsers(users);

            this.SpringCleanRoles(roles);
            this.SpringCleanUsers(users);
        }

        private void SeedRoles(RoleManager<ApplicationRole> roles)
        {
            var roleInfo = new (string name, string[] perms)[]
            {
                (Constants.Roles.SuperAdmin, Permissions.AllPermissions)
            };

            foreach (var info in roleInfo)
            {
                var role = new ApplicationRole
                {
                    Name = info.name,
                    NormalizedName = info.name.ToUpper()
                };

                if (!roles.RoleExistsAsync(role.Name).Result)
                    roles.CreateAsync(role).Wait();
                else
                    role = roles.FindByNameAsync(role.Name).Result;

                // Always try to add new permissions
                var roleClaims = roles.GetClaimsAsync(role).Result;
                foreach (var perm in info.perms)
                {
                    var claim = new Claim(Permissions.ClaimType, perm);

                    if (!roleClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                        roles.AddClaimAsync(role, claim).Wait();
                }
            }
        }

        private void SpringCleanRoles(RoleManager<ApplicationRole> roles)
        {
            foreach (var role in roles.Roles.ToList())
            {
                // Remove any permissions that no longer exist.
                var roleClaims = roles.GetClaimsAsync(role).Result;
                var invalidPermClaims = this.GetInvalidPermissionClaims(roleClaims);
                foreach (var claim in invalidPermClaims)
                    roles.RemoveClaimAsync(role, claim).Wait();
            }
        }

        private void SeedUsers(UserManager<ApplicationUser> users)
        {
            var existingUser = users.FindByEmailAsync("admin@example.com").Result;
            if (existingUser != null)
                return;

            var user = new ApplicationUser
            {
                Email = "admin@example.com",
                EmailConfirmed = true,
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                UserName = DEFAULT_USERNAME
            };

            user.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(user, DEFAULT_PASSWORD);

            // Create default superadmin
            users.CreateAsync(user).Wait();
            users.AddToRoleAsync(user, Constants.Roles.SuperAdmin).Wait();
        }

        private void SpringCleanUsers(UserManager<ApplicationUser> users)
        {
            foreach (var user in users.Users.ToList())
            {
                // Remove any permissions that no longer exist.
                var userClaims = users.GetClaimsAsync(user).Result;
                var invalidPermClaims = this.GetInvalidPermissionClaims(userClaims);
                foreach (var claim in invalidPermClaims)
                    users.RemoveClaimAsync(user, claim).Wait();
            }

        }

        private IEnumerable<Claim> GetInvalidPermissionClaims(IEnumerable<Claim> claims)
        {
            return claims.Where(c => c.Type == Permissions.ClaimType)
                         .Where(c => !Permissions.AllPermissions.Contains(c.Value));
        }
    }

    public class IdentityContextFactory : FarmMasterContextFactory<IdentityContext>
    {
    }
}
