using DataAccess;
using DataAccessGraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessGraphQL
{
    public class DataAccessRootQuery : ObjectGraphType<object>
    {
        private DataAccessUserContext _context;
        private FarmMasterContext     _farmMaster;
        private IdentityContext       _identity;

        public DataAccessRootQuery(
            IHttpContextAccessor         context, 
            UserManager<ApplicationUser> users,
            GraphQLUserContextAccessor   accessor,
            FarmMasterContext            fmDb,
            IdentityContext              idDb)
        {
            this._context    = accessor.Context;
            this._farmMaster = fmDb;
            this._identity   = idDb;

            var user         = users.GetUserAsync(context.HttpContext.User).Result;
            accessor.Context = new DataAccessUserContext(user, context.HttpContext.User);
            if(user == null)
                throw new Exception("You are not logged in");

            this.AddUserQuery();
        }

        private void AddUserQuery()
        {
            FieldAsync<UserGraphType>(
                "user",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "username",
                        Description = "Get a user by their username"
                    }
                ),
                resolve: async ctx => 
                {
                    var username = ctx.GetArgument<string>("username");

                    return await this._identity.Users.FirstAsync(u => u.UserName == username);
                }
            );
        }
    }
}
