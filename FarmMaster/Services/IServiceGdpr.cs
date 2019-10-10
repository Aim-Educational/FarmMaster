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
        void AnonymiseContactData(Contact contact);
        void AnonymiseUserData(User user);
    }

    public interface IServiceGdpr
    {
        JObject GetAggregatedDataForUser(User user);
        void AnonymiseUser(User user);
        void AnonymiseContact(Contact contact);
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

        public void AnonymiseUser(User user)
        {
            this.cacheServices();

            foreach(var service in ServiceGdprAggregator._gdprServiceCache)
                service.AnonymiseUserData(user);
        }

        public void AnonymiseContact(Contact contact)
        {
            this.cacheServices();

            foreach (var service in ServiceGdprAggregator._gdprServiceCache)
                service.AnonymiseContactData(contact);
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
