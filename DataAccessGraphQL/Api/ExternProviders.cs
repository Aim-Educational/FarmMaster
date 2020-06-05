using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessGraphQL.Api
{
    public interface IGraphQLQueryProvider
    {
        void AddQueries(RootGraphQLQuery rootQuery);
    }

    public interface IGraphQLMutationProvider
    {
        void AddMutations(RootGraphQLMutation rootMutation);
    }
}
