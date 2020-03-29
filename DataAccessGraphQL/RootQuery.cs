using DataAccess;
using DataAccess.Constants;
using DataAccessGraphQL.Constants;
using DataAccessGraphQL.GraphTypes;
using DataAccessGraphQL.RootResolvers;
using DataAccessGraphQL.Util;
using GraphQL;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessGraphQL
{
    public class DataAccessRootQuery : RootBase
    {
        readonly UserRootResolver _userResolver;
        readonly ContactRootResolver _contactResolver;
        readonly SignInManager<ApplicationUser> _signIn;

        public DataAccessRootQuery(
            // Required for RootBase
            IHttpContextAccessor         context, 
            UserManager<ApplicationUser> users,
            GraphQLUserContextAccessor   accessor,
            FarmMasterContext            fmDb,
            IdentityContext              idDb,
            IAuthorizationService        auth,

            // Custom
            UserRootResolver userResolver,
            ContactRootResolver contactResolver,
            SignInManager<ApplicationUser> signIn
        ) : base(context, users, accessor, fmDb, idDb, auth)
        {
            this._userResolver = userResolver;
            this._contactResolver = contactResolver;
            this._signIn = signIn;

            this.AddUserQuery();
            this.AddPermissionsQuery();
            this.AddContactQuery();
        }

        private void AddUserQuery()
        {
            FieldAsync<UserGraphType>(
                "user",
                arguments: this._userResolver,
                resolve: ctx => this._userResolver.ResolveAsync(ctx, base.DataContext)
            );

            this.DefineConnectionAsync<object, UserGraphType, DataAccessUserContext>(
                "users",
                base.DataContext,
                async (ctx, first, after) => 
                {
                    await ctx.EnforceHasPolicyAsync(Permissions.User.Read);
                    var users = await base.UserManager.Users
                                                      .Skip(after)
                                                      .Take(first)
                                                      .ToListAsync();
                    var contexts = users.Select(u => new DataAccessUserContext(u, this._signIn.CreateUserPrincipalAsync(u).Result, this.DataContext.Auth));

                    return contexts;
                }
            );
        }

        private void AddPermissionsQuery()
        {
            Field<ListGraphType<StringGraphType>>(
                "permissions",
                "All permissions accepted by GraphQL.",
                resolve: ctx => Permissions.AllPermissions
            );
        }

        private void AddContactQuery()
        {
            FieldAsync<ContactGraphType>(
                "contact",
                arguments: this._contactResolver,
                resolve: ctx => this._contactResolver.ResolveAsync(ctx, base.DataContext)
            );

            this.DefineConnectionAsync<object, ContactGraphType, Contact>(
                "contacts",
                base.DataContext,
                async (ctx, first, after) =>
                {
                    await ctx.EnforceHasPolicyAsync(Permissions.Contact.Read);
                    return await this._contactResolver
                                     .Manager
                                     .GetPageAsync(after, first);
                }
            );
        }
    }
}
