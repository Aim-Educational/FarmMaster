using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FarmMaster.Module.Core.Features
{
    public class OnConfigureServicesFeature : GenericContainerFeature<OnConfigureServices>
    {
    }

    public abstract class OnConfigureServices
    {
        public abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    }
}
