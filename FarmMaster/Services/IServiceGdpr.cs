using Business.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceGdprData
    {
        void GetContactGdprData(Contact contact, JObject json);
        void GetUserGdprData(User user, JObject json);
    }

    public interface IServiceGdpr
    {
        JObject GetAggregatedDataForUser(User user);
    }

    public class ServiceGdprAggregator : IServiceGdpr
    {
        readonly IServiceProvider _services;

        static IReadOnlyCollection<IServiceGdprData> _gdprServiceCache;

        public ServiceGdprAggregator(IServiceProvider services)
        {
            this._services = services;
        }

        public JObject GetAggregatedDataForUser(User user)
        {
            this.cacheServices();

            var json = new JObject();
            foreach(var service in ServiceGdprAggregator._gdprServiceCache)
                service.GetUserGdprData(user, json);

            return json;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        private void cacheServices()
        {
            if (ServiceGdprAggregator._gdprServiceCache == null)
            {
                ServiceGdprAggregator._gdprServiceCache
                    = Assembly.GetExecutingAssembly()
                              .DefinedTypes
                              .Where(t => t.ImplementedInterfaces.Contains(typeof(IServiceGdprData)))
                              .Select(t => this._services.GetService(t) as IServiceGdprData)
                              .Where(s => s != null)
                              .Distinct()
                              .ToList();
            }
        }
    }
}
