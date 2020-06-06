using CrudModule.GraphQL;
using DataAccess.Constants;
using FarmMaster.Module.Core;
using FarmMaster.Module.Core.Api;
using GraphQLModule.Api;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrudModule
{
    public class CrudGraphQLProvider : IApplicationFeatureProvider<GraphQLFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, GraphQLFeature feature)
        {
            feature.AddGraphQLPart<CrudModuleQueries>();
        }
    }

    public class Module : ModuleConfigurator
    {
        public override ModuleInfo Info => new ModuleInfo 
        {
            Name = "CrudModule"
        };

        public override void RegisterFeatureProviders(ApplicationPartManager parts)
        {
            parts.FeatureProviders.Add(new CrudGraphQLProvider());
        }

        public override void RegisterNavMenuItems(NavMenu menu)
        {
            menu.GroupFromName("Manage").AddRange(new[]
            {
                new NavMenuItem
                {
                    DisplayText      = "Breeds",
                    RequiredPolicies = new[]{ Permissions.Breed.ManageUI },
                    LinkHref         = new Uri("/Breed", UriKind.Relative)
                },
                new NavMenuItem
                {
                    DisplayText      = "Species",
                    RequiredPolicies = new[]{ Permissions.Species.ManageUI },
                    LinkHref         = new Uri("/Species", UriKind.Relative)
                },
                new NavMenuItem
                {
                    DisplayText      = "Contacts",
                    RequiredPolicies = new[]{ Permissions.Contact.ManageUI },
                    LinkHref         = new Uri("/Contact", UriKind.Relative)
                }
            });
        }
    }
}
