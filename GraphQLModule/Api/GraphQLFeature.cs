using DataAccessGraphQL;
using DataAccessGraphQL.Api;
using FarmMaster.Module.Core.Features;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQLModule.Api
{
    public class GraphQLFeature
    {
        private List<Type> _partTypes;
        public IEnumerable<Type> PartTypes => _partTypes;

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
