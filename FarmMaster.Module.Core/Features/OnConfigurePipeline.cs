using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace FarmMaster.Module.Core.Features
{
    public class OnConfigurePipelineFeature : GenericContainerFeature<OnConfigurePipeline>
    {
    }

    public abstract class OnConfigurePipeline
    {
        public abstract void Configure(IServiceProvider services, IWebHostEnvironment env);
    }
}
