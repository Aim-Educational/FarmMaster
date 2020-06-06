using DataAccess.Constants;
using FarmMaster.Module.Core;
using FarmMaster.Module.Core.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrudModule
{
    public class Module : ModuleConfigurator
    {
        public override ModuleInfo Info => new ModuleInfo 
        {
            Name = "CrudModule"
        };

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
