using DataAccess.Constants;
using FarmMaster.Module.Core;
using FarmMaster.Module.Core.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserModule
{
    public class Module : ModuleConfigurator
    {
        public override ModuleInfo Info => new ModuleInfo 
        {
            Name      = "UserModule",
            LoadOrder = 0
        };

        public override void RegisterNavMenuItems(NavMenu menu)
        {
            menu.GroupFromName("Admin").Add(new NavMenuItem
            {
                DisplayText      = "Users",
                RequiredPolicies = new[] { Permissions.User.ManageUI },
                LinkHref         = new Uri("/Admin/User/Users", UriKind.Relative)
            });
        }
    }
}
