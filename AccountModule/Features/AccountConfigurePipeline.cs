using EmailSender;
using FarmMaster.Module.Core.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using AccountModule.Constants;
using Microsoft.AspNetCore.Builder;

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
