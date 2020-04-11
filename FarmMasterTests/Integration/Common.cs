using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FarmMasterTests.Integration
{
    public static class Common
    {
        public static TestServer TestHost => new TestServer(
            new WebHostBuilder()
            .UseWebRoot("../../../../FarmMaster/wwwroot/")
            .ConfigureLogging(c => 
            {
                c.AddConsole().SetMinimumLevel(LogLevel.Trace);
                c.AddDebug();
            })
            .ConfigureAppConfiguration(c => 
            {
                // A few options to provide the FmTest section
                c.AddEnvironmentVariables();
                c.AddJsonFile(Path.GetFullPath("../../../../FarmMaster/appsettings.Development.json"));

                // For the Azure options
                c.AddJsonFile(Path.GetFullPath("../../../../FarmMaster/appsettings.json"));

                var config  = c.Build(); // So we can access env vars for the next part.
                var uuid    = Guid.NewGuid(); // So each instance has their own databases.
                var host    = config.GetValue<string>("FmTest:Host", "localhost");
                var port    = config.GetValue<string>("FmTest:Port", "5432");
                var user    = config.GetValue<string>("FmTest:User", "test");
                var pass    = config.GetValue<string>("FmTest:Pass", "test");

                c.AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "ConnectionStrings:Identity",   $"Host={host};Port={port};Database=IdentityTest{uuid};User Id={user};Password={pass}" },
                    { "ConnectionStrings:FarmMaster", $"Host={host};Port={port};Database=FarmMasterTest{uuid};User Id={user};Password={pass}" }
                });
            })
            .UseStartup<FarmMaster.Startup>()
        );
    }
}
