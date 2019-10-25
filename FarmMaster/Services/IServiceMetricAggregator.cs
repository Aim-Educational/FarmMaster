using Business.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceMetricAggregator
    {
        IEnumerable<MetricRequest> RequestMetrics { get; }

        MetricRequest OnStartHttpRequest(HttpContext context, bool doNotTrack = false);
        void OnEndHttpRequest(MetricRequest metric);
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

        public MetricRequest OnStartHttpRequest(HttpContext context, bool doNotTrack)
        {
            Contract.Requires(context != null);
            var request = new MetricRequest
            {
                DateTimeUtc      = DateTimeOffset.UtcNow,
                TraceIdentifier  = context.TraceIdentifier,
                Path             = context.Request.Path.Value,
                Ip               = (doNotTrack) ? "DNT" : context.Connection.RemoteIpAddress.ToString(),
                BytesUsedAtStart = GC.GetTotalMemory(false)
            };

            this._requestMetrics.Add(request);
            return request;
        }

        public void OnEndHttpRequest(MetricRequest metric)
        {
            Contract.Requires(metric != null);
            metric.BytesUsedAtEnd = GC.GetTotalMemory(false);
        }

        public void Reset()
        {
            var toKeep = this._requestMetrics.Where(m => m.BytesUsedAtEnd == 0).ToList();
            this._requestMetrics.Clear();
            this._requestMetrics.AddRange(toKeep);
        }
    }
}
