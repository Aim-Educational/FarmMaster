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
            // -> Find all classes that end in "RootResolver", except the actual RootResolver base class itself.
            var resolvers = thisAssembly
                            .DefinedTypes
                            .Where(t => t.IsClass)
                            .Where(t => t.Name.EndsWith("RootResolver"))
                            .Where(t => t != typeof(RootResolver<>)); // Don't want the actual base class included.
            
            foreach(var resolver in resolvers)
                services.AddScoped(resolver);

            return services;
        }
    }
}
