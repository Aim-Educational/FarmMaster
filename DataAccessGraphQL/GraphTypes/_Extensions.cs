using DataAccess.Constants;
using GraphQL;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessGraphQL.GraphTypes
{
    internal static class _Extensions
    {
        public static async Task EnforceHasPolicyAsync(this IAuthorizationService auth, ClaimsPrincipal user, string perm)
        {
            var result = await auth.AuthorizeAsync(user, perm);
            if (!result.Succeeded)
                throw new ExecutionError($"Missing permission {perm}");
        }
    }
}
