using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Constants;
using DataAccessGraphQL;
using DataAccessLogic;
using FarmMaster.Services;
using FarmMaster.Constants;
using FarmMaster.Services.Configuration;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FarmMaster
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Cookies
            services.Configure<CookiePolicyOptions>(o => 
            {
                o.CheckConsentNeeded = c => true;
                o.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Database
            services.AddDbContext<IdentityContext>(o => o.UseNpgsql(Configuration.GetConnectionString("Identity")));
            services.AddDbContext<FarmMasterContext>(o => o.UseNpgsql(Configuration.GetConnectionString("FarmMaster")));

            // Email
            services.AddScoped<IConfigureOptions<EmailSenderConfig>, ConfigureEmailOptions>();
            services.AddScoped<IEmailSender, EmailSender>();

            // Identity + All login providers
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
                Configuration.Bind("AzureAd", options);
                options.CookieSchemeName = IdentityConstants.ExternalScheme;
            });

            services.AddAuthorization(o => 
            {
                o.AddPolicy(Policies.IsAdmin, p => p.RequireRole(Roles.SuperAdmin));
                
                // For ease-of-use, all permissions have their own policy.
                // Any policy containing "read" (e.g. "read_permissions") will implicitly pass for their "write" version ("write_permissions")
                foreach(var perm in Permissions.AllPermissions)
                {
                    o.AddPolicy(
                        perm, 
                        p => p.RequireAssertion(
                            c => c.User.HasClaim(Permissions.ClaimType, perm)
                              && c.User.HasClaim(Permissions.ClaimType, perm.Replace("read", "write"))
                        )
                    );
                }
            });

            // GraphQL
            services.AddScoped<DataAccessGraphQLSchema>();
            services.AddGraphQL(o => 
            {
                o.EnableMetrics = false;
                o.ExposeExceptions = true;
            })
            .AddSystemTextJson()
            .AddGraphTypes(typeof(DataAccessGraphQLSchema), ServiceLifetime.Scoped);

            // MVC
            services.AddControllersWithViews()
                    .AddRazorRuntimeCompilation();
            services.AddRazorPages();
            services.AddRouting(o => 
            {
                o.LowercaseQueryStrings = false;
                o.LowercaseUrls = false;
            });

            // Misc
            services.AddDataAccessLogicLayer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            app.UseStatusCodePages();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseGraphQL<DataAccessGraphQLSchema>("/graphql");
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions
            {
                PlaygroundSettings = new Dictionary<string, object> {
                    { "request.credentials", "same-origin" } // Instructs the playground to send cookies, so we can validate the user.
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
