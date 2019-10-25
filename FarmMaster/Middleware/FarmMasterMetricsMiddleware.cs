using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FarmMaster.Middleware
{
    public class FarmMasterMetricsMiddleware
    {
        private readonly RequestDelegate _next;

        public FarmMasterMetricsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, IServiceMetricAggregator metrics)
        { 
            MetricRequest metric = null;
            try
            {
                // DNT = Do not track. 0 = fine. 1 = don't.
                // If DNT is true, anonymise the IP.
                metric = metrics.OnStartHttpRequest(httpContext, httpContext.Request.Headers["DNT"] == "1");
                return _next(httpContext);
            }
            finally
            {
                if(metric != null)
                    metrics.OnEndHttpRequest(metric);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class FarmMasterMetricsMiddlewareExtensions
    {
        public static IApplicationBuilder UseFarmMasterMetricsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FarmMasterMetricsMiddleware>();
        }
    }
}
