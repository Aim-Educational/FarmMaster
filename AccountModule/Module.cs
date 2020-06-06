using AccountModule.Features;
using AccountModule.GraphQL;
using FarmMaster.Module.Core;
using FarmMaster.Module.Core.Features;
using GraphQLModule.Api;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Collections.Generic;

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

    internal class AccountGraphQLProvider : IApplicationFeatureProvider<GraphQLFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, GraphQLFeature feature)
        {
            feature.AddGraphQLPart<AccountQueries>();
        }
    }

    public class Module : ModuleConfigurator
    {
        public override ModuleInfo Info => this._info;
        private readonly ModuleInfo _info = new ModuleInfo
        {
            Name = "AccountModule",
            LoadOrder = 0
        };

        public override void RegisterFeatureProviders(ApplicationPartManager parts)
        {
            parts.FeatureProviders.Add(new AccountConfigureProvider());
            parts.FeatureProviders.Add(new AccountGraphQLProvider());
        }
    }
}
