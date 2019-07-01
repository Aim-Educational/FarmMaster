using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Business.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using FarmMaster.Services;
using FarmMaster.Misc;

namespace FarmMaster
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // Cookie consent.
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

            // HSTS
            services.AddHsts(o => 
            {
                o.Preload = true;
                o.IncludeSubDomains = true;
                o.MaxAge = TimeSpan.FromDays(365);
            });

            // Database
            services.AddDbContext<FarmMasterContext>(o => o.UseSqlServer(Configuration.GetConnectionString("General")));

            // Auth
            services
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultSignInScheme       = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme    = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(o => 
            {
                o.ReturnUrlParameter = "redirectTo";
                o.LoginPath          = "/Account/Login";
            });

            // User manager
            services.AddScoped<IServiceUserManager, ServiceUserManager>();

            // SMTP
            services.Configure<IServiceSmtpClientConfig>(o =>
            {
                o.Host        = Configuration.GetValue<string>("Smtp:Host");
                o.Port        = Configuration.GetValue<ushort>("Smtp:Port");
                o.Credentials = new System.Net.NetworkCredential(
                    Configuration.GetValue<string>("Smtp:Username"),
                    Configuration.GetValue<string>("Smtp:Password")
                );
            });

            services.Configure<IServiceSmtpDomainConfig>(o =>
            {
                var domain = Configuration.GetValue<string>("AIMDEPLOY:DOMAIN");
                o.VerifyEmail = $"https://{domain}/Account/VerifyEmail?token=";
            });

            services.Configure<IServiceSmtpTemplateConfig>(o =>
            {
                o.EmailTemplates.Add(EnumEmailTemplateNames.EmailVerify, "/Views/EmailTemplates/EmailVerify.cshtml");
            });

            services.AddScoped<IServiceSmtpClient, ServiceSmtpClient>();
            services.AddScoped<IViewRenderService, ViewRenderService>();

            // MVC
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            (new FarmMasterContext(Configuration.GetConnectionString("Migrate")))
                .Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // For nginx
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
