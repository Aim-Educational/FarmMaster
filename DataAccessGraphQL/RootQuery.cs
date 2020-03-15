using DataAccess;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessGraphQL
{
    public class DataAccessRootQuery : ObjectGraphType<object>
    {
        public DataAccessRootQuery(IHttpContextAccessor context, UserManager<ApplicationUser> users)
        {
            Field<StringGraphType>("myName", resolve: ctx => users.GetUserAsync(context.HttpContext.User).Result.UserName);
        }
    }
}
