using FarmMaster.Module.Core;
using FarmMaster.Module.Core.Features;
using GraphQLModule.Features;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQLModule
{
    internal class GraphQLConfigureProvider : IApplicationFeatureProvider<ConfigureFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ConfigureFeature feature)
        {
            feature.ConfigurePipeline.Add(new GraphQLConfigurePipeline());
            feature.ConfigureServices.Add(new GraphQLConfigureServices());
        }
    }

    public class Module : ModuleConfigurator
    {
        public override ModuleInfo Info => _info;
        private readonly ModuleInfo _info = new ModuleInfo
        {
            Name      = "AccountModule",
            LoadOrder = int.MaxValue // Since other modules can register things with us, we need to stay at the bottom.
        };

        public override void RegisterFeatureProviders(ApplicationPartManager parts)
        {
            parts.FeatureProviders.Add(new GraphQLConfigureProvider());
        }
    }
}
