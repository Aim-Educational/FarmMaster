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
        public UserGraphType(GraphQLUserContextAccessor contextAccessor, IAuthorizationService auth)
        {
            var context = contextAccessor.Context;

            Field<StringGraphType>(
                "username",
                resolve: ctx => ctx.Source.UserName
            );
        }
    }
}
