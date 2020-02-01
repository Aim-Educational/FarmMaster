using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    /// <summary>
    /// Don't directly inherit this please.
    /// </summary>
    public interface IServiceHookConsumerBase
    {

    }

    public interface IServiceHookConsumer<T> : IServiceHookConsumerBase
    where T : class
    {
        void ConsumeHook(T hookData);
    }

    public interface IServiceHookEmitter
    {
        void Emit<T>(T hookData) where T : class;
    }

    public class ServiceHookEmitter : IServiceHookEmitter
    {
        readonly IServiceScopeFactory _scopeFactory;

        static IDictionary<Type, IEnumerable<Type>> _consumerCache;

        public ServiceHookEmitter(IServiceScopeFactory scopeFactory)
        {
            this._scopeFactory = scopeFactory;
            _consumerCache = new Dictionary<Type, IEnumerable<Type>>();
        }

        public void Emit<T>(T hookData) where T : class
        {            
            using(var scope = this._scopeFactory.CreateScope())
            {
                var consumers = this.GetAndCacheConsumerTypes<T>(scope.ServiceProvider);
                foreach(var consumer in consumers.Select(c => scope.ServiceProvider.GetRequiredService(c))
                                                 .Select(c => c as IServiceHookConsumer<T>)
                                                 .Where(c => c != null)
                )
                {
                    consumer.ConsumeHook(hookData);
                }
            }
        }

        private IEnumerable<Type> GetAndCacheConsumerTypes<T>(IServiceProvider services) where T : class
        {
            if(_consumerCache.ContainsKey(typeof(T)))
                return _consumerCache[typeof(T)];

            _consumerCache[typeof(T)]
                = Assembly.GetExecutingAssembly()
                          .DefinedTypes
                          .Where(t => t.ImplementedInterfaces.Contains(typeof(IServiceHookConsumer<T>)))
                          .Where(t => (services.GetService(t) as IServiceHookConsumer<T>) != null)
                          .Distinct()
                          .ToList();

            return _consumerCache[typeof(T)];
        }
    }
}
