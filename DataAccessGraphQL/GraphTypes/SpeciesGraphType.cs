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
    public class SpeciesGraphType : ObjectGraphType<Species>
    {
        public SpeciesGraphType(GraphQLUserContextAccessor contextAccessor)
        {
            var context = contextAccessor.Context;

            Field<StringGraphType>("id", resolve: ctx => ctx.Source.SpeciesId);

            FieldAsync<StringGraphType>(
                "name",
                resolve: async ctx => 
                {
                    await context.EnforceHasPolicyAsync(Permissions.Species.Read);
                    return ctx.Source.Name;
                }
            );

            FieldAsync<ListGraphType<NoteGraphType>>(
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
