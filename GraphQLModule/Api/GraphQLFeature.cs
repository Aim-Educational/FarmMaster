using DataAccessGraphQL;
using DataAccessGraphQL.Api;
using System;
using System.Collections.Generic;

namespace GraphQLModule.Api
{
    public class GraphQLFeature
    {
        private readonly List<Type> _partTypes;
        public IEnumerable<Type> PartTypes => this._partTypes;

        public GraphQLFeature()
        {
            this._partTypes = new List<Type>();
        }

        public void AddGraphQLPart<T>()
        where T : GraphQLPart
        {
            this._partTypes.Add(typeof(T));
        }
    }

    public abstract class GraphQLPart : IGraphQLQueryProvider, IGraphQLMutationProvider
    {
        public virtual void AddMutations(RootGraphQLMutation rootMutation)
        {
        }

        public virtual void AddQueries(RootGraphQLQuery rootQuery)
        {
        }
    }
}
