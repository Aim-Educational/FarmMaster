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
