using DataAccess;
using DataAccess.Constants;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessGraphQL.GraphTypes
{
    public class UserGraphType : ObjectGraphType<ApplicationUser>
    {
        public UserGraphType(GraphQLUserContextAccessor contextAccessor, FarmMasterContext db, IAuthorizationService auth)
        {
            var context = contextAccessor.Context;

            FieldAsync<StringGraphType>(
                "username",
                resolve: async ctx => 
                {
                    var user = ctx.Source as ApplicationUser;
                    await auth.EnforceHasPolicyAsync(context.UserPrincipal, Permissions.User.ReadPermissions);

                    return user.UserName;
                }
            );
        }
    }
}
