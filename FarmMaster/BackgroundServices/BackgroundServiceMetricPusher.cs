using Business.Model;
using FarmMaster.Services;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FarmMaster.BackgroundServices
{
    public class BackgroundServiceMetricPusher : IFarmBackgroundService
    {
        readonly FarmMasterContext _context;
        readonly IServiceMetricAggregator _metrics;

        public IFarmBackgroundServiceConfig Config => new IFarmBackgroundServiceConfig 
        { 
            DelayPerTicks = TimeSpan.FromMinutes(5),
            RestartOnException = true
        };

        public BackgroundServiceMetricPusher(
            FarmMasterContext context, 
            IServiceMetricAggregator metrics,
            IHostingEnvironment env)
        {
            this._context = context;
            this._metrics = metrics;

            // Commit metrics at a faster rate in a dev env, to make debugging easier.
            if(env.IsDevelopment())
                this.Config.DelayPerTicks = TimeSpan.FromSeconds(5);
        }

        public async Task OnShutdown()
        {
            await this.PushMetrics();
        }

        public async Task OnTickAsync(CancellationToken stoppingToken)
        {
            await this.PushMetrics();
        }

        private async Task PushMetrics()
        {
            foreach(var request in this._metrics.RequestMetrics)
                await this._context.AddAsync(request);

            this._metrics.Reset();
            await this._context.SaveChangesAsync();
        }
    }
}
