using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace FarmMaster.Middleware
{
    /**
     * So, apparently Identity is supposed to automatically add role claims to users automatically.
     * 
     * It doesn't.
     * 
     * So this exists now.
     * */
    public class AddRoleClaimsToUserMiddleware
    {
        private readonly RequestDelegate _next;

        public AddRoleClaimsToUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext httpContext, 
            UserManager<ApplicationUser> users,
            RoleManager<ApplicationRole> roles)
        {
            var user = await users.GetUserAsync(httpContext.User);
            if(user != null)
            {
                var identity = new ClaimsIdentity();
                foreach(var roleName in await users.GetRolesAsync(user))
                {
                    var roleObj = await roles.FindByNameAsync(roleName);
                    identity.AddClaims(await roles.GetClaimsAsync(roleObj));
                }

                httpContext.User.AddIdentity(identity);
            }

            await _next(httpContext);
        }
    }

    public static class AddRoleClaimsToUserMiddlewareExtensions
    {
        public static IApplicationBuilder UseAddRoleClaimsToUserMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AddRoleClaimsToUserMiddleware>();
        }
    }
}
