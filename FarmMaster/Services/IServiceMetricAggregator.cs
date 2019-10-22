using Business.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceMetricAggregator
    {
        IEnumerable<MetricRequest> RequestMetrics { get; }

        void OnHttpRequest(HttpContext context);
        void Reset();
    }

    public class ServiceMetricAggregator : IServiceMetricAggregator
    {
        private List<MetricRequest> _requestMetrics;
        public IEnumerable<MetricRequest> RequestMetrics { get => _requestMetrics; }

        public ServiceMetricAggregator()
        {
            this._requestMetrics = new List<MetricRequest>();
        }

        public void OnHttpRequest(HttpContext context)
        {
            var request = new MetricRequest
            {
                DateTimeUtc     = DateTimeOffset.UtcNow,
                TraceIdentifier = context.TraceIdentifier,
                Path            = context.Request.Path.Value,
                Ip              = context.Connection.RemoteIpAddress.ToString()
            };

            this._requestMetrics.Add(request);
        }

        public void Reset()
        {
            this._requestMetrics.Clear();
        }
    }
}
