using System;
using System.Collections.Generic;
using System.Text;

namespace FarmMaster.Module.Core.Features
{
    public class GenericContainerFeature<T>
    {
        public IList<T> Parts { get; private set; }

        public GenericContainerFeature()
        {
            this.Parts = new List<T>();
        }
    }
}
