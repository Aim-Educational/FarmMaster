using DataAccess;
using GraphQL.Types;

namespace DataAccessGraphQL.GraphTypes
{
    public class NoteGraphType : ObjectGraphType<NoteEntry>
    {
        public NoteGraphType(GraphQLUserContextAccessor contextAccessor)
        {
            var context = contextAccessor.Context;

            this.Field<StringGraphType>("id", resolve: ctx => ctx.Source.NoteEntryId);
            this.Field<StringGraphType>("category", resolve: ctx => ctx.Source.Category);
            this.Field<StringGraphType>("content", resolve: ctx => ctx.Source.Content);
        }
    }
}
