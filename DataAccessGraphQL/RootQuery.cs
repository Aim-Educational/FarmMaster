using DataAccess;
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
        readonly SignInManager<ApplicationUser> _signIn;

        public DataAccessRootQuery(
            IHttpContextAccessor         context, 
            UserManager<ApplicationUser> users,
            GraphQLUserContextAccessor   accessor,
            FarmMasterContext            fmDb,
            IdentityContext              idDb,
            IAuthorizationService        auth,

            UserRootResolver userResolver,
            SignInManager<ApplicationUser> sigIn
        ) : base(context, users, accessor, fmDb, idDb, auth)
        {
            this._userResolver = userResolver;
            this._signIn = sigIn;

            this.AddUserQuery();
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
                async (first, after) => 
                {
                    var users    = await base.UserManager.Users
                                                         .Skip(after)
                                                         .Take(first)
                                                         .ToListAsync();
                    var contexts = users.Select(u => new DataAccessUserContext(u, this._signIn.CreateUserPrincipalAsync(u).Result, this.DataContext.Auth));

                    return contexts;
                }
            );
        }
    }
}
