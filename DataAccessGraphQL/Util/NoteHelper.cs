using DataAccess;
using DataAccess.Constants;
using DataAccessLogic;
using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessGraphQL.Util
{
    public delegate NoteOwner GetNoteOwnerFunc<TSourceType>(IResolveFieldContext<TSourceType> ctx);

    public static class NoteHelper
    {
        public static void AddNoteMutationsAsync<TSourceType>(
            this ObjectGraphType<TSourceType> mutator,
            DataAccessUserContext context,
            IUnitOfWork unitOfWork,
            INoteManager notes,
            string writeNotesPerm,
            GetNoteOwnerFunc<TSourceType> getNoteOwner
        )
        where TSourceType : class
        {
            mutator.FieldAsync<BooleanGraphType>(
                "addNote",
                "Adds a note.",
                new QueryArguments(
                    // category: String!
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "category"
                    },
                    // content: String!
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "content"
                    }
                ),
                async ctx =>
                {
                    await context.EnforceHasPolicyAsync(writeNotesPerm);

                    using (var scope = unitOfWork.Begin("Add note"))
                    {
                        var owner = getNoteOwner(ctx);
                        var note = new NoteEntry
                        {
                            NoteOwner = owner,
                            Category = ctx.GetArgument<string>("category"),
                            Content = ctx.GetArgument<string>("content")
                        };
                        await notes.CreateAsync(note);
                        scope.Commit();
                    }

                    return true;
                }
            );

            mutator.FieldAsync<BooleanGraphType>(
                "deleteNotes",
                "Deletes a set of notes.",
                new QueryArguments(
                    // ids: [ID!]!
                    new QueryArgument<NonNullGraphType<ListGraphType<NonNullGraphType<IdGraphType>>>>
                    {
                        Name = "ids"
                    }
                ),
                async ctx =>
                {
                    await context.EnforceHasPolicyAsync(writeNotesPerm);

                    using (var scope = unitOfWork.Begin("Delete notes"))
                    {
                        var owner = getNoteOwner(ctx);
                        foreach (var id in ctx.GetArgument<List<int>>("ids"))
                        {
                            var note = await notes.GetByIdAsync(id);
                            if (!note.Succeeded)
                            {
                                scope.Rollback("Note was not found");
                                return false;
                            }

                            if (!owner.NoteEntries.Any(e => e.NoteEntryId == note.Value.NoteEntryId))
                            {
                                scope.Rollback("Note does not belong to contact");
                                return false;
                            }

                            notes.Delete(note.Value);
                        }

                        scope.Commit();
                    }

                    return true;
                }
            );
        }
    }
}
