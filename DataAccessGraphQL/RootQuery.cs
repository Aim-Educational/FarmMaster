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
        private ApplicationUser _user;
        private UserManager<ApplicationUser> _users;

        public DataAccessRootQuery(IHttpContextAccessor context, UserManager<ApplicationUser> users)
        {
            this._user = users.GetUserAsync(context.HttpContext.User).Result;
            this._users = users;
        }
    }
}
