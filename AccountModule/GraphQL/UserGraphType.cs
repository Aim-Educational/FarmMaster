using DataAccess.Constants;
using DataAccessGraphQL;
using GraphQL.Types;
using System.Collections.Generic;
using System.Linq;

namespace AccountModule.GraphQL
{
    public class UserGraphType : ObjectGraphType<DataAccessUserContext>
    {
        public UserGraphType(GraphQLUserContextAccessor contextAccessor)
        {
            var context = contextAccessor.Context;

            this.Field<StringGraphType>(
                "username",
                resolve: ctx => ctx.Source.UserIdentity.UserName
            );

            this.FieldAsync<ListGraphType<StringGraphType>>(
                "permissions",
                resolve: async ctx =>
                {
                    await context.EnforceHasPolicyAsync(Permissions.User.ReadPermissions);
                    return ctx.Source
                              .UserPrincipal
                              .Claims
                              .Where(c => c.Type == Permissions.ClaimType)
                              .Select(c => c.Value)
                              .Distinct();
                }
            );
        }
    }
}
