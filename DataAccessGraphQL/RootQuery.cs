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
            ContactRootResolver contactResolver
        ) : base(context, users, accessor, fmDb, idDb, auth)
        {
            this._userResolver = userResolver;
            this._contactResolver = contactResolver;

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
                (ctx, first, after, order) => this._userResolver.ResolvePageAsync(ctx, first, after, order)
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
                (ctx, first, after, order) => this._contactResolver.ResolvePageAsync(base.DataContext, first, after, order)
            );
        }
    }
}
