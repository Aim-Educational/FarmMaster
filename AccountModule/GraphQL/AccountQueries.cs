using DataAccessGraphQL;
using DataAccessGraphQL.RootResolvers;
using GraphQLModule.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountModule.GraphQL
{
    public class AccountQueries : GraphQLPart
    {
        readonly RootResolver<DataAccessUserContext> _resolver;

        public AccountQueries(RootResolver<DataAccessUserContext> resolver)
        {
            this._resolver = resolver;
        }

        public override void AddQueries(RootGraphQLQuery rootQuery)
        {
            rootQuery.DefineSingleAndConnection<UserGraphType, DataAccessUserContext>("user", this._resolver);
        }

        public override void AddMutations(RootGraphQLMutation rootMutation)
        {
            rootMutation.AddGenericMutationAsync<UserRootMutation, DataAccessUserContext>("user", this._resolver);
        }
    }
}
