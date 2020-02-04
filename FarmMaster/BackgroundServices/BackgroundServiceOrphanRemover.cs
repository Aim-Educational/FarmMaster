using Business.Model;
using FarmMaster.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FarmMaster.BackgroundServices
{
    public class BackgroundServiceOrphanRemover : IFarmBackgroundService
    {
        readonly FarmMasterContext _context;

        public IFarmBackgroundServiceConfig Config => new IFarmBackgroundServiceConfig 
        { 
            DelayPerTicks = TimeSpan.FromMinutes(60),
            RestartOnException = true
        };

        public BackgroundServiceOrphanRemover(
            FarmMasterContext context
        )
        {
            this._context = context;
        }

        public Task OnShutdown()
        {
            return Task.CompletedTask;
        }

        public async Task OnTickAsync(CancellationToken stoppingToken)
        {
            this._context.RemoveRange(
                this._context
                    .LifeEventEntries
                    .Include(e => e.AnimalMap)
                    .Where(e => e.AnimalMap.Count() == 0)
                    .Where(e => (DateTimeOffset.UtcNow - e.DateTimeCreated) > TimeSpan.FromMinutes(2)) // Prevents edge case where we delete an entry just before any mappings are made.
            );
            this._context.SaveChanges();
        }
    }
}
