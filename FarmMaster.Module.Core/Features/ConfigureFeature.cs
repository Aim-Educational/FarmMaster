using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace FarmMaster.Module.Core.Features
{
    public class ConfigureFeature
    {
        public IList<ConfigureServicesPart> ConfigureServices { get; private set; }
        public IList<ConfigurePipelinePart> ConfigurePipeline { get; private set; }

        public ConfigureFeature()
        {
            this.ConfigureServices = new List<ConfigureServicesPart>();
            this.ConfigurePipeline = new List<ConfigurePipelinePart>();
        }
    }

    public abstract class ConfigureServicesPart
    {
        public abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration, ApplicationPartManager appParts);
    }

    public abstract class ConfigurePipelinePart
    {
        public abstract void Configure(IApplicationBuilder app, IServiceProvider services, IWebHostEnvironment env);
    }
}
