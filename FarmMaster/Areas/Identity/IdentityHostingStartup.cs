using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(FarmMaster.Areas.Identity.IdentityHostingStartup))]
namespace FarmMaster.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}