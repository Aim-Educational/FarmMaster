using AccountModule.Constants;
using DataAccess;
using DataAccess.Constants;
using FarmMaster.Module.Core.Features;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AccountModule.Features
{
    public class AccountConfigureServices : ConfigureServicesPart
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration, ApplicationPartManager appParts)
        {
            services.AddAntiforgery(o => o.HeaderName = "X-CSRF-TOKEN");
            services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
            {
                o.SignIn.RequireConfirmedAccount = true;

                o.Password.RequiredLength = 6;
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = false;

                o.Lockout.AllowedForNewUsers = true;
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                o.Lockout.MaxFailedAccessAttempts = 5;

                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(o =>
            {
                o.Cookie.HttpOnly = true;
                o.ExpireTimeSpan = TimeSpan.FromHours(4);

                o.LoginPath = "/Account/Login";
                o.LogoutPath = "/Account/Logout";
                o.AccessDeniedPath = "/Identity/Account/AccessDenied";
                o.SlidingExpiration = true;
            });

            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                options.Authority += "/v2.0/";
                options.TokenValidationParameters.ValidateIssuer = false;
            });

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddAzureAD(options =>
            {
                configuration.Bind("AzureAd", options);
                options.CookieSchemeName = IdentityConstants.ExternalScheme;
            });

            services.AddAuthorization(o =>
            {
                o.AddPolicy(Policies.IsAdmin, p => p.RequireRole(Roles.SuperAdmin));
                o.AddPolicy(
                    Policies.SeeAdminPanel,
                    p => p.RequireAssertion
                    (
                        ctx => ctx.User.IsInRole(Roles.SuperAdmin)
                            || ctx.User.HasClaim(Permissions.ClaimType, Permissions.Other.GraphQLUI)
                            || ctx.User.HasClaim(Permissions.ClaimType, Permissions.Other.DebugUI)
                            || ctx.User.HasClaim(Permissions.ClaimType, Permissions.Other.Settings)
                            || ctx.User.HasClaim(Permissions.ClaimType, Permissions.User.ManageUI)
                    )
                );

                // For ease-of-use, all permissions have their own policy.
                // Any policy containing "read" (e.g. "read_permissions") will implicitly pass for their "write" version ("write_permissions")
                foreach (var perm in Permissions.AllPermissions)
                {
                    o.AddPolicy(
                        perm,
                        p => p.RequireClaim(Permissions.ClaimType, perm, perm.Replace("read", "write"))
                    );
                }
            });
        }
    }
}
