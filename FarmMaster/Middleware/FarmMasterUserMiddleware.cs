using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FarmMaster.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FarmMaster.Middleware
{
    public class FarmMasterUserMiddleware
    {
        private readonly RequestDelegate _next;

        public FarmMasterUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext,
                           IOptions<CookieAuthenticationOptions> options,
                           IServiceUserManager users)
        {
            if(!httpContext.User.Identity.IsAuthenticated)
                return _next(httpContext);
            
            var user = users.UserFromCookieSession(httpContext);
            if(user == null)
            {
                httpContext.Response.Redirect(options.Value.LoginPath);
                httpContext.SignOutAsync();
                httpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // SignOutAsync doesn't reset IsAuthenticated, so we just make an empty Principal for this request.
                return _next(httpContext);
            }

            if(!user.UserPrivacy.HasVerifiedEmail)
            {
                if(!httpContext.Request.Path.Value.StartsWith("/Account/Login")
                && !httpContext.Request.Path.Value.StartsWith("/Account/Signup")
                && !httpContext.Request.Path.Value.StartsWith("/Account/VerifyEmail")
                && !httpContext.Request.Path.Value.StartsWith("/Account/ResendEmailVerifyEmail")
                )
                    httpContext.Response.Redirect("/Account/Login?verifyEmail=true");
                return _next(httpContext);
            }

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class FarmMasterUserMiddlewareExtensions
    {
        public static IApplicationBuilder UseFarmMasterUserMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FarmMasterUserMiddleware>();
        }
    }
}
