using DataAccess;
using DataAccess.Constants;
using DataAccessLogic;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessGraphQL.Mutations
{
    public class ContactRootMutation : ObjectGraphType<Contact>
    {
        readonly DataAccessUserContext _context;
        readonly IContactManager       _contacts;
        readonly INoteManager          _notes;
        readonly IUnitOfWork           _unitOfWork;

        public ContactRootMutation(
            GraphQLUserContextAccessor   accessor,
            IContactManager              contacts,
            INoteManager                 notes,
            IUnitOfWork                  unitOfWork
        )
        {
            this._context       = accessor.Context;
            this._contacts      = contacts;
            this._notes         = notes;
            this._unitOfWork    = unitOfWork;

            this.AddNoteMutations();
        }

        private void AddNoteMutations()
        {
            FieldAsync<BooleanGraphType>(
                "addNote",
                "Adds a note.",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> 
                    {
                        Name = "category"
                    },
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "content"
                    }
                ),
                async ctx => 
                {
                    using(var scope = this._unitOfWork.Begin("Add note"))
                    {
                        ctx.Source.NoteOwner = ctx.Source.NoteOwner ?? new NoteOwner();
                        this._contacts.Update(ctx.Source);

                        var note = new NoteEntry 
                        {
                            NoteOwner = ctx.Source.NoteOwner,
                            Category  = ctx.GetArgument<string>("category"),
                            Content   = ctx.GetArgument<string>("content")
                        };
                        await this._notes.CreateAsync(note);
                        scope.Commit();
                    }

                    return true;
                }
            );

            FieldAsync<BooleanGraphType>(
                "deleteNote",
                "Deletes a note.",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> 
                    {
                        Name = "id"
                    }
                ),
                async ctx => 
                {
                    using(var scope = this._unitOfWork.Begin("Delete note"))
                    {
                        ctx.Source.NoteOwner = ctx.Source.NoteOwner ?? new NoteOwner();
                        this._contacts.Update(ctx.Source);

                        var note = await this._notes.GetByIdAsync(ctx.GetArgument<int>("id"));
                        if(!note.Succeeded)
                        {
                            scope.Rollback("Note was not found");
                            return false;
                        }

                        if(!ctx.Source.NoteOwner.NoteEntries.Any(e => e.NoteEntryId == note.Value.NoteEntryId))
                        {
                            scope.Rollback("Note does not belong to contact");
                            return false;
                        }

                        this._notes.Delete(note.Value);
                        scope.Commit();
                    }

                    return true;
                }
            );
        }
    }
}
