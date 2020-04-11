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
            this.Query = ServiceProviderExtensions.GetRequiredService<DataAccessRootQuery>(services);
            this.Mutation = ServiceProviderExtensions.GetRequiredService<DataAccessRootMutation>(services);
        }
    }
}
