using DataAccess;
using DataAccess.Constants;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace DataAccessGraphQL.GraphTypes
{
    public class NoteGraphType : ObjectGraphType<NoteEntry>
    {
        public NoteGraphType(GraphQLUserContextAccessor contextAccessor)
        {
            var context = contextAccessor.Context;

            Field<StringGraphType>("id",       resolve: ctx => ctx.Source.NoteEntryId);
            Field<StringGraphType>("category", resolve: ctx => ctx.Source.Category);
            Field<StringGraphType>("content",  resolve: ctx => ctx.Source.Content);
        }
    }
}
