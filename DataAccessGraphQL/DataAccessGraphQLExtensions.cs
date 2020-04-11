using DataAccessGraphQL.RootResolvers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessGraphQL
{
    public static class DataAccessGraphQLExtensions
    {
        public static IServiceCollection AddDataAccessGraphQLSchema(this IServiceCollection services)
        {
            return services.AddScoped<UserRootResolver>()
                           .AddScoped<ContactRootResolver>()
                           .AddScoped<SpeciesRootResolver>()
                           .AddScoped<DataAccessGraphQLSchema>()
                           .AddScoped<GraphQLUserContextAccessor>();
        }
    }
}
