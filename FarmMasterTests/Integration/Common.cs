using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FarmMasterTests.Integration
{
    public static class Common
    {
        public static TestServer TestHost => new TestServer(
            new WebHostBuilder()
            .UseWebRoot("../../../../FarmMaster/wwwroot/")
            .ConfigureAppConfiguration(c => 
            {
                c.AddEnvironmentVariables();

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
