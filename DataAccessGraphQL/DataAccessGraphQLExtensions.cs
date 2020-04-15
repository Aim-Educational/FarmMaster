using DataAccessGraphQL.RootResolvers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessGraphQL
{
    public static class DataAccessGraphQLExtensions
    {
        public static IServiceCollection AddDataAccessGraphQLSchema(this IServiceCollection services)
        {
            return services.AddRootResolvers()
                           .AddScoped<DataAccessGraphQLSchema>()
                           .AddScoped<GraphQLUserContextAccessor>();
        }

        static IServiceCollection AddRootResolvers(this IServiceCollection services)
        {
            // Get the DataAccessGraphQL assembly, as we're gonna inspect all classes to automatically
            // register the root resolvers.
            var thisAssembly = typeof(DataAccessGraphQLExtensions).Assembly;

            //    Get all types in DataAccessGraphQL
            // -> Find all non-abstract classes that end in "RootResolver".
            var resolvers = thisAssembly
                            .DefinedTypes
                            .Where(t => t.IsClass)
                            .Where(t => !t.IsAbstract)
                            .Where(t => t.Name.EndsWith("RootResolver"));
            
            foreach(var resolver in resolvers)
            {
                // If the resolver implements RootResolver, then register it as RootResolver<Type>.
                // Otherwise register it as the resolver's type.

                var baseType = resolver.BaseType;

                // Rewrite CrudRootResolver<T> -> RootResolver<T>
                if(baseType.GetGenericTypeDefinition() == typeof(CrudRootResolver<>))
                    baseType = typeof(RootResolver<>).MakeGenericType(baseType.GetGenericArguments().First());

                if(baseType.GetGenericTypeDefinition() != typeof(RootResolver<>))
                {
                    services.AddScoped(resolver);
                    continue;
                }

                services.AddScoped(baseType, resolver);
            }

            return services;
        }
    }
}
