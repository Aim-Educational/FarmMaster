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
        public IdentityContext(DbContextOptions<IdentityContext> context) : base(context)
        {
        }
    }

    class IdentityContextFactory : FarmMasterContextFactory<IdentityContext>
    {
    }
}
