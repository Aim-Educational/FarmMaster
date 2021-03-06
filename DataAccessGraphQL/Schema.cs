﻿using GraphQL.Types;
using GraphQL.Utilities;
using System;

namespace DataAccessGraphQL
{
    public class DataAccessGraphQLSchema : Schema
    {
        public DataAccessGraphQLSchema(IServiceProvider services)
            : base(services)
        {
            this.Query = ServiceProviderExtensions.GetRequiredService<RootGraphQLQuery>(services);
            this.Mutation = ServiceProviderExtensions.GetRequiredService<RootGraphQLMutation>(services);
        }
    }
}
