using GraphQL.Types;
using GraphQL.Utilities;
using System;

namespace DataAccessGraphQL
{
    public class DataAccessGraphQLSchema : Schema
    {
        public DataAccessGraphQLSchema(IServiceProvider services) 
            : base(services)
        {
            this.Query = this.Services.GetRequiredService<DataAccessRootQuery>();
        }
    }
}
