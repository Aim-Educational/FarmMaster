using DataAccess.Constants;
using DataAccessGraphQL;
using DataAccessGraphQL.RootResolvers;
using GraphQL.Types;
using GraphQLModule.Api;

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
            rootQuery.Field<ListGraphType<StringGraphType>>(
                "permissions",
                "All permissions accepted by GraphQL.",
                resolve: ctx => Permissions.AllPermissions
            );
        }

        public override void AddMutations(RootGraphQLMutation rootMutation)
        {
            rootMutation.AddGenericMutationAsync<UserRootMutation, DataAccessUserContext>("user", this._resolver);
        }
    }
}
