using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            // DNT = Do not track. 0 = fine. 1 = don't.
            if(httpContext.Request.Headers["DNT"] == "1")
                metrics.OnHttpRequest(httpContext);
            return _next(httpContext);
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
