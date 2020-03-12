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
    }

    public class IdentityContextFactory : FarmMasterContextFactory<IdentityContext>
    {
    }
}
