using EmailSender;
using FarmMaster.Module.Core.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using AccountModule.Constants;

namespace AccountModule.Features
{
    public class AccountConfigurePipeline : OnConfigurePipeline
    {
        public override void Configure(IServiceProvider services, IWebHostEnvironment env)
        {
            env.LoadTemplates();
        }
    }
}
