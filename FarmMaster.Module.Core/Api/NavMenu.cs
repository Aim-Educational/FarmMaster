using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmMaster.Module.Core.Api
{
    public sealed class NavMenu : List<NavMenuGroup>
    {
        public NavMenuGroup GroupFromName(string name)
        {
            var result = this.FirstOrDefault(g => g.DisplayText == name);

            if(result == null)
            {
                result = new NavMenuGroup() { DisplayText = name };
                this.Add(result);
            }

            return result;
        }
    }

    public sealed class NavMenuGroup : List<NavMenuItem>
    {
        public string DisplayText { get; set; }
    }

    public sealed class NavMenuItem
    {
        public IList<string> RequiredPolicies { get; set; }
        public string DisplayText { get; set; }
        public Uri LinkHref { get; set; }
        public Uri IconSrc { get; set; }

        public NavMenuItem()
        {
            this.RequiredPolicies = new List<string>();
        }
    }
}
