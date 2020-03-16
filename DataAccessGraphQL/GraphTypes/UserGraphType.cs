using DataAccess;
using DataAccess.Constants;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessGraphQL.GraphTypes
{
    public class UserGraphType : ObjectGraphType<ApplicationUser>
    {
        public UserGraphType(GraphQLUserContextAccessor contextAccessor)
        {
            var context = contextAccessor.Context;

            Field<StringGraphType>(
                "username",
                resolve: ctx => ctx.Source.UserName
            );

            FieldAsync<ListGraphType<StringGraphType>>(
                "permissions",
                resolve: async ctx => 
                {
                    await context.EnforceHasPolicyAsync(Permissions.User.ReadPermissions);
                    return context.UserPrincipal
                                  .Claims
                                  .Where(c => c.Type == Permissions.ClaimType)
                                  .Select(c => c.Value)
                                  .Distinct();
                }
            );
        }
    }
}
