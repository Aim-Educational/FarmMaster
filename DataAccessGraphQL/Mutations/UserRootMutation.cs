using DataAccess;
using DataAccess.Constants;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessGraphQL.Mutations
{
    public class UserRootMutation : ObjectGraphType<DataAccessUserContext>
    {
        readonly UserManager<ApplicationUser> _users;
        readonly RoleManager<ApplicationRole> _roles;
        readonly DataAccessUserContext        _context;

        public UserRootMutation(
            GraphQLUserContextAccessor   accessor, 
            UserManager<ApplicationUser> users,
            RoleManager<ApplicationRole> roles
        )
        {
            this._context = accessor.Context;
            this._users   = users;
            this._roles   = roles;

            this.AddSetPermissions();
        }

        private void AddSetPermissions()
        {
            FieldAsync<ListGraphType<StringGraphType>>(
                "setPermissions",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ListGraphType<StringGraphType>>>()
                    {
                        Name = "permissions",
                        Description = "A list of permission names to add to the user."
                    }
                ),
                resolve: async ctx => 
                {
                    await this._context.EnforceHasPolicyAsync(Permissions.User.WritePermissions);

                    // Perm filtering
                    var claims          = ctx.Source.UserPrincipal.Claims;
                    var permsFromParams = ctx.GetArgument<List<string>>("permissions");
                    var permsCurrent    = claims.Where(c => c.Type == Permissions.ClaimType).Select(c => c.Value);
                    var permsToAdd      = permsFromParams.Where(p => !permsCurrent.Contains(p));
                    var permsInvalid    = permsFromParams.Where(p => !Permissions.AllPermissions.Contains(p));

                    if(permsInvalid.Any())
                    {
                        var permsErrorString = permsInvalid.Aggregate((a, b) => a + ";" + b);
                        throw new ExecutionError($"The following permissions do not exist: {permsErrorString}");
                    }

                    await this._users.AddClaimsAsync(
                        ctx.Source.UserIdentity, 
                        permsToAdd.Select(p => new Claim(Permissions.ClaimType, p))
                    );

                    return permsToAdd;
                }
            );
        }
    }
}
