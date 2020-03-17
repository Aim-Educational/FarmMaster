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
            var arguments = new QueryArguments
            (
                new QueryArgument<NonNullGraphType<ListGraphType<StringGraphType>>>()
                {
                    Name = "permissions",
                    Description = "A list of permission names."
                }
            );

            FieldAsync<ListGraphType<StringGraphType>>(
                "grantPermissions",
                arguments: arguments,
                resolve: async ctx => 
                {
                    await this._context.EnforceHasPolicyAsync(Permissions.User.WritePermissions);

                    // Perm filtering
                    var perms      = this.GetValidCurrentPerms(ctx);
                    var permsToAdd = perms.fromParams.Where(p => !perms.current.Contains(p));

                    await this._users.AddClaimsAsync(
                        ctx.Source.UserIdentity, 
                        permsToAdd.Select(p => new Claim(Permissions.ClaimType, p))
                    );

                    return permsToAdd;
                }
            );

            FieldAsync<ListGraphType<StringGraphType>>(
                "revokePermissions",
                arguments: arguments,
                resolve: async ctx => 
                {
                    await this._context.EnforceHasPolicyAsync(Permissions.User.ReadPermissions);

                    var perms         = this.GetValidCurrentPerms(ctx);
                    var permsToRemove = perms.fromParams.Where(p => perms.current.Contains(p));

                    await this._users.RemoveClaimsAsync(
                        ctx.Source.UserIdentity,
                        permsToRemove.Select(p => new Claim(Permissions.ClaimType, p))
                    );

                    return permsToRemove;
                }
            );
        }

        private (IEnumerable<string> current, IEnumerable<string> fromParams) 
        GetValidCurrentPerms(IResolveFieldContext<DataAccessUserContext> permContext)
        {
            var claims          = permContext.Source.UserPrincipal.Claims;
            var permsFromParams = permContext.GetArgument<List<string>>("permissions");
            var permsCurrent    = claims.Where(c => c.Type == Permissions.ClaimType).Select(c => c.Value);
            var permsInvalid    = permsFromParams.Where(p => !Permissions.AllPermissions.Contains(p));

            if (permsInvalid.Any())
            {
                var permsErrorString = permsInvalid.Aggregate((a, b) => a + ";" + b);
                throw new ExecutionError($"The following permissions do not exist: {permsErrorString}");
            }

            return (permsCurrent, permsFromParams);
        }
    }
}
