using DataAccess.Constants;
using FarmMaster.Module.Core;
using FarmMaster.Module.Core.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdminModule
{
    public class Module : ModuleConfigurator
    {
        public override ModuleInfo Info => new ModuleInfo 
        {
            Name      = "AdminModule",
            LoadOrder = 0
        };

        public override void RegisterNavMenuItems(NavMenu menu)
        {
            menu.GroupFromName("Admin").AddRange(new[]
            {
                new NavMenuItem
                {
                    DisplayText         = "ControlTest",
                    RequiredPolicies    = new[]{ Permissions.Other.DebugUI },
                    LinkHref            = new Uri("/Admin/ControlTest", UriKind.Relative)
                },

                new NavMenuItem
                {
                    DisplayText         = "Settings",
                    RequiredPolicies    = new[]{ Permissions.Other.Settings },
                    LinkHref            = new Uri("/Admin/Settings", UriKind.Relative)
                }
            });
        }
    }
}
