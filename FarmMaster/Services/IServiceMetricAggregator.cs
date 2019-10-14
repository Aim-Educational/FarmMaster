using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceMetricAggregator
    {
        RequestMetrics RequestMetrics { get; }

        void BumpRequestCount();
        void Reset();
    }

    public class ServiceMetricAggregator : IServiceMetricAggregator
    {
        public RequestMetrics RequestMetrics { get; private set; }

        public ServiceMetricAggregator()
        {
            this.RequestMetrics = new RequestMetrics();
        }

        public void BumpRequestCount()
        {
            this.RequestMetrics.Count++;
        }

        public void Reset()
        {
            this.RequestMetrics.Count = 0;
        }
    }
    
    public class RequestMetrics
    {
        public uint Count { get; internal set; }
    }
}
