using DataAccess;
using DataAccess.Constants;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessGraphQL.RootResolvers
{
    public class UserRootResolver : RootResolver
    {
        readonly UserManager<ApplicationUser>   _users;
        readonly SignInManager<ApplicationUser> _signIn;
        readonly RoleManager<ApplicationRole>   _roles;

        public UserRootResolver(
            UserManager<ApplicationUser> users,
            SignInManager<ApplicationUser> signIn,
            RoleManager<ApplicationRole> roles
        )
        {
            this._users  = users;
            this._signIn = signIn;
            this._roles  = roles;

            base.Add(new QueryArgument<NonNullGraphType<StringGraphType>>
            {
                Name = "username",
                Description = "Get a user by their username"
            });
        }

        public override async Task<object> ResolveAsync(
            IResolveFieldContext<object> context, 
            DataAccessUserContext userContext
        )
        {
            await userContext.EnforceHasPolicyAsync(Permissions.User.Read);

            // ARGS
            var username = context.GetArgument<string>("username");

            // Find the user
            var user = await this._users.FindByNameAsync(username);
            if(user == null)
                throw new ExecutionError($"No user called {username} was found");

            var principal = await this._signIn.CreateUserPrincipalAsync(user);

            // We need access to both the Identity entity, as well as all our claims that we can't easily get because see above.
            return new DataAccessUserContext(user, principal, userContext.Auth);
        }
    }
}
