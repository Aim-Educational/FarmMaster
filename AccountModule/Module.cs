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
    internal class AccountConfigureProvider : IApplicationFeatureProvider<ConfigureFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ConfigureFeature feature)
        {
            feature.ConfigurePipeline.Add(new AccountConfigurePipeline());
            feature.ConfigureServices.Add(new AccountConfigureServices());
        }
    }

    public class Module : ModuleConfigurator
    {
        public override ModuleInfo Info => _info;
        private readonly ModuleInfo _info = new ModuleInfo 
        {
            Name      = "AccountModule",
            LoadOrder = 0
        };

        public override void RegisterFeatureProviders(ApplicationPartManager parts)
        {
            parts.FeatureProviders.Add(new AccountConfigureProvider());
        }
    }
}
