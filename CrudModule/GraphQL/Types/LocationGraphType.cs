using DataAccess;
using DataAccess.Constants;
using DataAccessGraphQL;
using DataAccessGraphQL.GraphTypes;
using GraphQL.Types;

namespace CrudModule.GraphQL.Types
{
    public class LocationGraphType : ObjectGraphType<Location>
    {
        public LocationGraphType(GraphQLUserContextAccessor contextAccessor)
        {
            var context = contextAccessor.Context;

            this.Field<StringGraphType>("id", resolve: ctx => ctx.Source.LocationId);

            this.FieldAsync<StringGraphType>(
                "name",
                resolve: async ctx =>
                {
                    await context.EnforceHasPolicyAsync(Permissions.Location.Read);
                    return ctx.Source.Name;
                }
            );

            this.FieldAsync<ListGraphType<NoteGraphType>>(
                "notes",
                resolve: async ctx =>
                {
                    await context.EnforceHasPolicyAsync(Permissions.Location.ReadNotes);
                    return ctx.Source.NoteOwner?.NoteEntries;
                }
            );

            this.FieldAsync<StringGraphType>(
                "type",
                resolve: async ctx => 
                {
                    await context.EnforceHasPolicyAsync(Permissions.Location.Read);
                    return ctx.Source.Type;
                }
            );
        }
    }
}
