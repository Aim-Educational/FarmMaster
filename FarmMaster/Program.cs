using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FarmMaster
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            SetupDatabase(host.Services);
            host.Run();
        }

        public static void SetupDatabase(IServiceProvider provider)
        {
            // Identity doesn't really provide a place to seed data properly, so we have to do this.
            // May as well handle FarmMasterContext here as well.
            using (var scope = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db    = scope.ServiceProvider.GetRequiredService<IdentityContext>();
                var fmdb  = scope.ServiceProvider.GetRequiredService<FarmMasterContext>();
                var roles = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                db.Database.Migrate();
                db.Seed(roles, users);

                fmdb.Database.Migrate();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}