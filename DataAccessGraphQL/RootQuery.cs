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

        private void DefineSingleAndConnection<TGraphType, TSourceType>(string name, RootResolver<TSourceType> resolver)
        where TGraphType : GraphType
        where TSourceType : class
        {
            var namePlural = name + "s";

            FieldAsync<TGraphType>(
                name,
                arguments: resolver,
                resolve: ctx => resolver.ResolveAsync(ctx, base.DataContext)
            );

            this.DefineConnectionAsync<object, TGraphType, TSourceType>(
                namePlural,
                base.DataContext,
                (ctx, first, after, order) => resolver.ResolvePageAsync(ctx, first, after, order)
            );
        }

        private void AddUserQuery()
        {
            this.DefineSingleAndConnection<UserGraphType, DataAccessUserContext>("user", this._userResolver);
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
            this.DefineSingleAndConnection<ContactGraphType, Contact>("contact", this._contactResolver);
        }
    }
}
