using AccountModule.Features;
using FarmMaster.Module.Core;
using FarmMaster.Module.Core.Features;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountModule
{
    public class ConfigureServicesProvider : IApplicationFeatureProvider<OnConfigureServicesFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, OnConfigureServicesFeature feature)
        {
            feature.Features.Add(new AccountConfigureServices());
        }
    }

    public class Module : ModuleConfigurator
    {
        public override void RegisterFeatureProviders(ApplicationPartManager parts)
        {
            parts.FeatureProviders.Add(new ConfigureServicesProvider());
        }
    }
}
