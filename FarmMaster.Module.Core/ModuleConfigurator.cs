using FarmMaster.Module.Core.Api;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace FarmMaster.Module.Core
{
    public sealed class ModuleInfo
    {
        public string Name { get; set; }
        public int LoadOrder { get; set; }
    }

    public abstract class ModuleConfigurator
    {
        public abstract ModuleInfo Info { get; }
        public virtual void RegisterFeatureProviders(ApplicationPartManager parts) { }
        public virtual void RegisterNavMenuItems(NavMenu menu) { }
    }
}
