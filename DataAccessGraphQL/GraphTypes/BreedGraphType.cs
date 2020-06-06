using DataAccess;
using DataAccess.Constants;
using GraphQL.Types;

namespace DataAccessGraphQL.GraphTypes
{
    public class BreedGraphType : ObjectGraphType<Breed>
    {
        public BreedGraphType(GraphQLUserContextAccessor contextAccessor)
        {
            var context = contextAccessor.Context;

            this.Field<StringGraphType>("id", resolve: ctx => ctx.Source.BreedId);

            this.FieldAsync<StringGraphType>(
                "name",
                resolve: async ctx =>
                {
                    await context.EnforceHasPolicyAsync(Permissions.Breed.Read);
                    return ctx.Source.Name;
                }
            );

            this.FieldAsync<SpeciesGraphType>(
                "species",
                resolve: async ctx =>
                {
                    // SpeciesGraphType will handle the species permissions.
                    await context.EnforceHasPolicyAsync(Permissions.Breed.Read);
                    return ctx.Source.Species;
                }
            );

            this.FieldAsync<ListGraphType<NoteGraphType>>(
                "notes",
                resolve: async ctx =>
                {
                    await context.EnforceHasPolicyAsync(Permissions.Breed.ReadNotes);
                    return ctx.Source.NoteOwner?.NoteEntries;
                }
            );
        }
    }
}
