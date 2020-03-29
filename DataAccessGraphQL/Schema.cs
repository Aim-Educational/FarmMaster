using DataAccess;
using DataAccessGraphQL.GraphTypes;
using DataAccessGraphQL.RootResolvers;
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
            this.Mutation = GraphQL.Utilities.ServiceProviderExtensions.GetRequiredService<DataAccessRootMutation>(services);
        }
    }

    public static class Extensions
    {
        public static IServiceCollection AddDataAccessGraphQLSchema(this IServiceCollection services)
        {
            return services.AddScoped<UserRootResolver>()
                           .AddScoped<ContactRootResolver>()
                           .AddScoped<DataAccessGraphQLSchema>()
                           .AddScoped<GraphQLUserContextAccessor>();
        }
    }
}
