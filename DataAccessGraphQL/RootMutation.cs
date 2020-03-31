using DataAccess;
using DataAccessGraphQL.GraphTypes;
using DataAccessGraphQL.Mutations;
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
    // Meh, code duplication between query and mutation.
    public class DataAccessRootMutation : RootBase
    {
        readonly UserRootResolver _userResolver;
        readonly ContactRootResolver _contactResolver;

        public DataAccessRootMutation(
            IHttpContextAccessor         context, 
            UserManager<ApplicationUser> users,
            GraphQLUserContextAccessor   accessor,
            FarmMasterContext            fmDb,
            IdentityContext              idDb,
            IAuthorizationService        auth,

            UserRootResolver userResolver,
            ContactRootResolver contactResolver
        ) : base(context, users, accessor, fmDb, idDb, auth)
        {
            this._userResolver = userResolver;
            this._contactResolver = contactResolver;

            this.AddUserMutation();
            this.AddContactMutation();
        }

        private void AddUserMutation()
        {
            FieldAsync<UserRootMutation>(
                "user", 
                arguments: this._userResolver, 
                resolve: ctx => this._userResolver.ResolveAsync(ctx, base.DataContext)
            );
        }

        private void AddContactMutation()
        {
            FieldAsync<ContactRootMutation>(
                "contact",
                arguments: this._contactResolver,
                resolve: ctx => this._contactResolver.ResolveAsync(ctx, base.DataContext)
            );
        }
    }
}
