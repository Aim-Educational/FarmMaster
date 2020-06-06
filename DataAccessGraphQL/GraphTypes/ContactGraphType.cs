﻿using DataAccess;
using DataAccess.Constants;
using GraphQL.Types;

namespace DataAccessGraphQL.GraphTypes
{
    public class ContactGraphType : ObjectGraphType<Contact>
    {
        public ContactGraphType(GraphQLUserContextAccessor contextAccessor)
        {
            var context = contextAccessor.Context;

            this.Field<StringGraphType>("id", resolve: ctx => ctx.Source.ContactId);

            this.FieldAsync<StringGraphType>(
                "name",
                resolve: async ctx =>
                {
                    await context.EnforceHasPolicyAsync(Permissions.Contact.Read);
                    return ctx.Source.Name;
                }
            );

            this.FieldAsync<ListGraphType<NoteGraphType>>(
                "notes",
                resolve: async ctx =>
                {
                    await context.EnforceHasPolicyAsync(Permissions.Contact.ReadNotes);
                    return ctx.Source.NoteOwner?.NoteEntries;
                }
            );
        }
    }
}
