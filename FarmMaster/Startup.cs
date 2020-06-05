using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Constants;
using DataAccessGraphQL;
using DataAccessLogic;
using FarmMaster.Services;
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
using FarmMaster.Middleware;
using EmailSender;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using FarmMaster.Module.Core;
using FarmMaster.Module.Core.Features;

namespace FarmMaster
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
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
            services.AddSingleton<IConfigureOptions<EmailSenderConfig>, ConfigureEmailOptions>();
            services.AddTemplatedEmailSender();

            // GraphQL
            services.AddDataAccessGraphQLSchema();
            services.AddGraphQL(o => 
            {
                o.EnableMetrics = false;
                o.ExposeExceptions = true;
            })
            .AddSystemTextJson()
            .AddGraphTypes(typeof(DataAccessGraphQLSchema), ServiceLifetime.Scoped);

            // MVC & Modules & Run OnConfigureServices features
            services.AddControllersWithViews()
                    .AddFarmMasterBuiltinModules(services, this.WebHostEnvironment)
                    .AddRazorRuntimeCompilation();
            services.AddRazorPages();
            services.AddRouting(o => 
            {
                o.LowercaseQueryStrings = false;
                o.LowercaseUrls = false;
            });

            var provider = services.BuildServiceProvider();
            var appParts = provider.GetRequiredService<ApplicationPartManager>();
            var feature  = new ConfigureFeature();
            appParts.PopulateFeature(feature);
            
            foreach(var configFeature in feature.ConfigureServices)
                configFeature.ConfigureServices(services, Configuration);

            // Misc
            services.AddDataAccessLogicLayer();
            services.AddSingleton<FarmLoggerProvider>();
        }

        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env, 
            ILoggerFactory loggerFactory,
            IServiceProvider services,
            FarmLoggerProvider farmProvider,
            ApplicationPartManager parts
        )
        {
            var configFeature = new ConfigureFeature();
            parts.PopulateFeature(configFeature);

            loggerFactory.AddProvider(farmProvider);

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
            app.UseAddRoleClaimsToUserMiddleware();
            app.UseAuthorization();

            foreach(var module in configFeature.ConfigurePipeline)
                module.Configure(services, this.WebHostEnvironment);

            app.UseGraphQL<DataAccessGraphQLSchema>("/graphql");
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions
            {
                PlaygroundSettings = new Dictionary<string, object> {
                    { "request.credentials", "same-origin" } // Instructs the playground to send cookies, so we can validate the user.
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("defaultArea", "{area:exists}/{controller=Home}/{action=Index}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }

    public static class FarmMasterModuleExtensions
    {
        public static IMvcBuilder AddFarmMasterBuiltinModules(
            this IMvcBuilder builder, 
            IServiceCollection services,
            IWebHostEnvironment env
        )
        {
            var modules = new List<(Assembly assembly, string hotReloadDir)>
            {
                (typeof(AccountModule.Controllers.ModuleController).Assembly, "AccountModule")
            };

            foreach(var assembly in modules.Select(m => m.assembly))
                builder.AddApplicationPart(assembly);

            #if DEBUG
            services.Configure<MvcRazorRuntimeCompilationOptions>(o => 
            {
                // This code breaks during unittests, but we don't need it then, so just skip this step.
                if(AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.ToLower().Contains("xunit")))
                    return;

                var basePath = Path.Combine(env.ContentRootPath, "..");
                foreach(var module in modules)
                    o.FileProviders.Add(new PhysicalFileProvider(Path.Combine(basePath, module.hotReloadDir)));
            });
            #endif

            builder.ConfigureApplicationPartManager(o => 
            {
                foreach (var assembly in modules.Select(m => m.assembly))
                {
                    var configuratorType = assembly.GetTypes()
                                                   .First(t => typeof(ModuleConfigurator).IsAssignableFrom(t));

                    var configuratorInstance = (ModuleConfigurator)Activator.CreateInstance(configuratorType);
                    configuratorInstance.RegisterFeatureProviders(o);
                }
            });

            return builder;
        }
    }
}
