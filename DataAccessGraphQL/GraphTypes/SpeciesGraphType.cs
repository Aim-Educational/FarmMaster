using DataAccess;
using DataAccess.Constants;
using GraphQL.Types;

namespace DataAccessGraphQL.GraphTypes
{
    public class SpeciesGraphType : ObjectGraphType<Species>
    {
        public SpeciesGraphType(GraphQLUserContextAccessor contextAccessor)
        {
            var context = contextAccessor.Context;

            this.Field<StringGraphType>("id", resolve: ctx => ctx.Source.SpeciesId);

            this.FieldAsync<StringGraphType>(
                "name",
                resolve: async ctx =>
                {
                    await context.EnforceHasPolicyAsync(Permissions.Species.Read);
                    return ctx.Source.Name;
                }
            );

            this.FieldAsync<ListGraphType<NoteGraphType>>(
                "notes",
                resolve: async ctx =>
                {
                    await context.EnforceHasPolicyAsync(Permissions.Species.ReadNotes);
                    return ctx.Source.NoteOwner?.NoteEntries;
                }
            );
        }
    }
}
