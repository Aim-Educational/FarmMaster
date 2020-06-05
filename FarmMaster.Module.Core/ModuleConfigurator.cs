using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
