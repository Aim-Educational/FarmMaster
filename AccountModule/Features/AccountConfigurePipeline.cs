using AccountModule.Constants;
using FarmMaster.Module.Core.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace AccountModule.Features
{
    public class AccountConfigurePipeline : ConfigurePipelinePart
    {
        public override void Configure(IApplicationBuilder app, IServiceProvider services, IWebHostEnvironment env)
        {
            env.LoadTemplates();
        }
    }
}
