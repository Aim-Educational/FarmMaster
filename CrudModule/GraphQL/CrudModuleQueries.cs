using CrudModule.GraphQL.Mutations;
using CrudModule.GraphQL.Types;
using DataAccess;
using DataAccessGraphQL;
using DataAccessGraphQL.RootResolvers;
using GraphQLModule.Api;

namespace CrudModule.GraphQL
{
    public class CrudModuleQueries : GraphQLPart
    {
        readonly RootResolver<Contact> _contactResolver;
        readonly RootResolver<Species> _speciesResolver;
        readonly RootResolver<Breed>   _breedResolver;

        public CrudModuleQueries(
            RootResolver<Contact> contactResolver,
            RootResolver<Species> speciesResolver,
            RootResolver<Breed> breedResolver
        )
        {
            this._contactResolver = contactResolver;
            this._speciesResolver = speciesResolver;
            this._breedResolver = breedResolver;
        }

        public override void AddQueries(RootGraphQLQuery rootQuery)
        {
            rootQuery.DefineSingleAndConnection<ContactGraphType, Contact>("contact", this._contactResolver);
            rootQuery.DefineSingleAndConnection<SpeciesGraphType, Species>("species", this._speciesResolver);
            rootQuery.DefineSingleAndConnection<BreedGraphType, Breed>    ("breed", this._breedResolver);
        }

        public override void AddMutations(RootGraphQLMutation rootMutation)
        {
            rootMutation.AddGenericMutationAsync<ContactRootMutation, Contact>("contact", this._contactResolver);
            rootMutation.AddGenericMutationAsync<SpeciesRootMutation, Species>("species", this._speciesResolver);
            rootMutation.AddGenericMutationAsync<BreedRootMutation, Breed>    ("breed", this._breedResolver);
        }
    }
}
