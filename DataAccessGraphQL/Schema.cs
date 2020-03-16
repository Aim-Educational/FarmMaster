using DataAccess;
using DataAccessGraphQL.GraphTypes;
using GraphQL.Types;
using GraphQL.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DataAccessGraphQL
{
    public class DataAccessGraphQLSchema : Schema
    {
        public DataAccessGraphQLSchema(IServiceProvider services) 
            : base(services)
        {
            this.Query = GraphQL.Utilities.ServiceProviderExtensions.GetRequiredService<DataAccessRootQuery>(services);
        }
    }

    public static class Extensions
    {
        public static IServiceCollection AddDataAccessGraphQLSchema(this IServiceCollection services)
        {
            return services.AddScoped<DataAccessGraphQLSchema>()
                           .AddScoped<GraphQLUserContextAccessor>();
        }
    }
}
