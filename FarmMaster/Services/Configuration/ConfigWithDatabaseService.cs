using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services.Configuration
{
    public abstract class ConfigWithDatabaseService<T> : IConfigureOptions<T>
        where T : class
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ConfigWithDatabaseService(IServiceScopeFactory serviceScopeFactory)
        {
            this._serviceScopeFactory = serviceScopeFactory;
        }

        public void Configure(T options)
        {
            using(var scope = this._serviceScopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<FarmMasterContext>();
                this.ConfigWithDatabase(options, db);
            }
        }

        protected abstract void ConfigWithDatabase(T options, FarmMasterContext db);
    }
}
