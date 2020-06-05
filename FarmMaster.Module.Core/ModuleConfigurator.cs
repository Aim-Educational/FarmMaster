using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace FarmMaster.Module.Core
{
    public abstract class ModuleConfigurator
    {
        public virtual void RegisterFeatureProviders(ApplicationPartManager parts) { }
    }
}
