using DataAccessLogic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace FarmMasterTests.Integration
{
    /// <summary>
    /// Contains common variables for all tests.
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// Creates and returns a new <see cref="TestServer"/> for FarmMaster.
        /// </summary>
        /// <remarks>
        /// By using either appsettings.Development.json, or env vars, you can provide the following variables:
        /// 
        /// FmTest:Host - The url to the test server. (Default: localhost)
        /// 
        /// FmTest:Port - The port to use. (Default: 5432)
        /// 
        /// FmTest:User - The username to use. (Default: test)
        /// 
        /// FmTest:Pass - The password to use. (Default: test)
        /// 
        /// FmTest:Smtp:User - The username for SMTP. (Default: )
        /// 
        /// FmTest:Smtp:Pass - The password for SMTP. (Default: )
        /// 
        /// FmTest:Smtp:Server - The server for SMTP. (Default: localhost)
        /// 
        /// FmTest:Smtp:Port - The port for SMTP. (Default: 5025)
        /// 
        /// Note that the user needs permissions to create and delete their own database.
        /// </remarks>
        public static TestServer TestHost => new TestServer(
            new WebHostBuilder()
            .UseWebRoot("../../../../FarmMaster/wwwroot/")
            .ConfigureLogging(c =>
            {
                c.AddConsole();
                c.AddDebug();

                c.AddFilter("Microsoft.EntityFrameworkCore.ChangeTracking", LogLevel.None);
            })
            .ConfigureAppConfiguration(c =>
            {
                // A few options to provide the FmTest section
                c.AddEnvironmentVariables();
                c.AddJsonFile(Path.GetFullPath("../../../../FarmMaster/appsettings.Development.json"));

                // For the Azure options
                c.AddJsonFile(Path.GetFullPath("../../../../FarmMaster/appsettings.json"));

                var config = c.Build(); // So we can access env vars for the next part.
                var uuid = Guid.NewGuid(); // So each instance has their own databases.
                var host = config.GetValue<string>("FmTest:Host", "localhost");
                var port = config.GetValue<string>("FmTest:Port", "5432");
                var user = config.GetValue<string>("FmTest:User", "test");
                var pass = config.GetValue<string>("FmTest:Pass", "test");

                c.AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "ConnectionStrings:Identity",   $"Host={host};Port={port};Database=IdentityTest{uuid};User Id={user};Password={pass}" },
                    { "ConnectionStrings:FarmMaster", $"Host={host};Port={port};Database=FarmMasterTest{uuid};User Id={user};Password={pass}" }
                });
            })
            .UseStartup<FarmMaster.Startup>()
            .ConfigureTestServices(c =>
            {
                // Setup the databases
                var provider = c.BuildServiceProvider();
                FarmMaster.Program.SetupDatabase(provider);

                // Set the SMTP settings.
                var config = provider.GetRequiredService<IConfiguration>();
                var settings = provider.GetRequiredService<IFarmMasterSettingsAccessor>();
                var newSettings = settings.Settings;

                newSettings.SmtpPassword = config.GetValue("FmTest:Smtp:Pass", "");
                newSettings.SmtpUsername = config.GetValue("FmTest:Smtp:User", "");
                newSettings.SmtpServer = config.GetValue("FmTest:Smtp:Server", "localhost");
                newSettings.SmtpPort = config.GetValue<ushort>("FmTest:Smtp:Port", 5025);

                settings.Settings = newSettings;
            })
            .UseStaticWebAssets()
        );
    }
}
