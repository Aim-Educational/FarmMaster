using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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

            // Identity doesn't really provide a place to seed data properly, so we have to do this.
            using(var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db    = scope.ServiceProvider.GetRequiredService<IdentityContext>();
                var roles = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                db.Seed(roles);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}