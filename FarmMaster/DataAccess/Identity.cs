using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    class ApplicationUser : IdentityUser<int>
    {
    }

    class ApplicationUserClaim : IdentityUserClaim<int>
    {
    }

    class ApplicationUserLogin : IdentityUserLogin<int>
    {
    }

    class ApplicationRole : IdentityRole<int>
    { 
    }

    class ApplicationRoleClaim : IdentityRoleClaim<int>
    {
    }

    class ApplicationUserRole : IdentityUserRole<int>
    { 
    }

    class ApplicationUserToken : IdentityUserToken<int>
    {
    }

    class IdentityContext : IdentityDbContext<
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

    }

    class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
    {
        public IdentityContext CreateDbContext(string[] args)
        {
            throw new NotImplementedException("TODO:");
        }
    }
}
