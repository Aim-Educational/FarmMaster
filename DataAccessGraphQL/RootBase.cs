using DataAccess;
using DataAccessGraphQL.GraphTypes;
using DataAccessGraphQL.RootResolvers;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessGraphQL
{
    // Trust me, I hate this almost as much as you do.
    public abstract class RootBase : ObjectGraphType<object>
    {
        protected DataAccessUserContext        DataContext  { private set; get; }
        protected FarmMasterContext            FarmMasterDb { private set; get; }
        protected IdentityContext              IdentityDb   { private set; get; }
        protected UserManager<ApplicationUser> UserManager  { private set; get; }

        public RootBase(
            IHttpContextAccessor            context,
            UserManager<ApplicationUser>    users,
            GraphQLUserContextAccessor      accessor,
            FarmMasterContext               fmDb,
            IdentityContext                 idDb,
            IAuthorizationService           auth
        )
        {
            var user = users.GetUserAsync(context.HttpContext.User).Result;
            accessor.Context = new DataAccessUserContext(user, context.HttpContext.User, auth);
            if (user == null)
                throw new Exception("You are not logged in");

            this.DataContext  = accessor.Context;
            this.FarmMasterDb = fmDb;
            this.IdentityDb   = idDb;
            this.UserManager  = users;
        }
    }
}
