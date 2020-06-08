using DataAccess.Constants;
using FarmMaster.Module.Core;
using FarmMaster.Module.Core.Api;
using FarmMaster.Module.Core.Features;
using GraphQLModule.Features;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;

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
        public override ModuleInfo Info => this._info;
        private readonly ModuleInfo _info = new ModuleInfo
        {
            Name = "AccountModule",
            LoadOrder = int.MaxValue // Since other modules can register things with us, we need to stay at the bottom.
        };

        public override void RegisterFeatureProviders(ApplicationPartManager parts)
        {
            parts.FeatureProviders.Add(new GraphQLConfigureProvider());
        }

        public override void RegisterNavMenuItems(NavMenu menu)
        {
            menu.GroupFromName("Admin").Add(new NavMenuItem
            {
                DisplayText = "GraphQL",
                RequiredPolicies = new[] { Permissions.Other.GraphQLUI },
                LinkHref = new Uri("/ui/playground", UriKind.Relative)
            });
        }
    }
}
