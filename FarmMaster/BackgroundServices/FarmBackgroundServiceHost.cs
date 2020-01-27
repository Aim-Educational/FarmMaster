﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FarmMaster.BackgroundServices
{
    // Only to be provided by the service itself, not via DI.
    public class IFarmBackgroundServiceConfig
    {
        public TimeSpan DelayPerTicks;
        public bool RestartOnException;
    }

    public interface IFarmBackgroundService
    {
        Task OnTickAsync(CancellationToken stoppingToken);
        Task OnShutdown();
        IFarmBackgroundServiceConfig Config { get; }
    }

    public class FarmBackgroundServiceHost<T> : BackgroundService 
    where T : IFarmBackgroundService 
    {
        readonly IServiceScopeFactory _serviceScope;
        readonly ILogger<FarmBackgroundServiceHost<T>> _logger;
        readonly IApplicationLifetime _lifetime;

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
        public FarmBackgroundServiceHost(
            IServiceScopeFactory serviceScope,
            ILogger<FarmBackgroundServiceHost<T>> logger,
            IApplicationLifetime lifetime
        )
        {
            this._serviceScope = serviceScope;
            this._logger = logger;
            this._lifetime = lifetime;

            // Graceful exit.
            lifetime.ApplicationStopping.Register(() => 
            {
                using (var scope = this._serviceScope.CreateScope())
                {
                    var service = ActivatorUtilities.CreateInstance<T>(scope.ServiceProvider);
                    service.OnShutdown().Wait();
                }
            });
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                this._logger.LogTrace("Tick");

                using (var scope = this._serviceScope.CreateScope())
                {
                    var service = ActivatorUtilities.CreateInstance<T>(scope.ServiceProvider);

                    try
                    {
                        await service.OnTickAsync(stoppingToken);
                    }
                    catch(Exception ex)
                    {
                        this._logger.LogError($"!!! SERVICE THREW EXCEPTION !!!\n{ex.Message}\n\n{ex.StackTrace}");
                        if(!service.Config.RestartOnException)
                            return;
                    }

                    await Task.Delay((int)service.Config.DelayPerTicks.TotalMilliseconds, stoppingToken);
                }
            }
        }
    }
}