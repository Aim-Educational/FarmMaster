using DataAccessGraphQL.RootResolvers;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace DataAccessGraphQL
{
    public static class DataAccessGraphQLExtensions
    {
        public static IServiceCollection AddDataAccessGraphQLSchema(this IServiceCollection services)
        {
            return services.AddRootResolvers(typeof(DataAccessGraphQLExtensions).Assembly)
                           .AddScoped<DataAccessGraphQLSchema>()
                           .AddScoped<GraphQLUserContextAccessor>();
        }

        public static IServiceCollection AddRootResolvers(this IServiceCollection services, Assembly assembly)
        {
            //    Get all types in the assembly
            // -> Find all non-abstract classes that end in "RootResolver".
            var resolvers = assembly
                            .DefinedTypes
                            .Where(t => t.IsClass)
                            .Where(t => !t.IsAbstract)
                            .Where(t => t.Name.EndsWith("RootResolver"));

            foreach (var resolver in resolvers)
            {
                // If the resolver implements RootResolver, then register it as RootResolver<Type>.
                // Otherwise register it as the resolver's type.

                var baseType = resolver.BaseType;

                // Rewrite CrudRootResolver<T> -> RootResolver<T>
                if (baseType.GetGenericTypeDefinition() == typeof(CrudRootResolver<>))
                    baseType = typeof(RootResolver<>).MakeGenericType(baseType.GetGenericArguments().First());

                if (baseType.GetGenericTypeDefinition() != typeof(RootResolver<>))
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
