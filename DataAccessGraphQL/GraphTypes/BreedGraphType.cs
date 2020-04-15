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
    public class BreedGraphType : ObjectGraphType<Breed>
    {
        public BreedGraphType(GraphQLUserContextAccessor contextAccessor)
        {
            var context = contextAccessor.Context;

            Field<StringGraphType>("id", resolve: ctx => ctx.Source.BreedId);

            FieldAsync<StringGraphType>(
                "name",
                resolve: async ctx => 
                {
                    await context.EnforceHasPolicyAsync(Permissions.Breed.Read);
                    return ctx.Source.Name;
                }
            );

            FieldAsync<SpeciesGraphType>(
                "species",
                resolve: async ctx =>
                {
                    // SpeciesGraphType will handle the species permissions.
                    await context.EnforceHasPolicyAsync(Permissions.Breed.Read);
                    return ctx.Source.Species;
                }
            );

            FieldAsync<ListGraphType<NoteGraphType>>(
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
