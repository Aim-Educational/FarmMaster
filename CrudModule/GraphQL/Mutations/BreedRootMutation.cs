using DataAccess;
using DataAccess.Constants;
using DataAccessGraphQL;
using DataAccessGraphQL.Util;
using DataAccessLogic;
using GraphQL.Types;

namespace CrudModule.GraphQL.Mutations
{
    public class BreedRootMutation : ObjectGraphType<Breed>
    {
        readonly DataAccessUserContext _context;
        readonly IBreedManager _breeds;
        readonly INoteManager _notes;
        readonly IUnitOfWork _unitOfWork;

        public BreedRootMutation(
            GraphQLUserContextAccessor accessor,
            IBreedManager breeds,
            INoteManager notes,
            IUnitOfWork unitOfWork
        )
        {
            this._context = accessor.Context;
            this._breeds = breeds;
            this._notes = notes;
            this._unitOfWork = unitOfWork;

            this.AddNoteMutations();
        }

        private void AddNoteMutations()
        {
            this.AddNoteMutationsAsync(
                this._context,
                this._unitOfWork,
                this._notes,
                Permissions.Breed.WriteNotes,
                ctx =>
                {
                    ctx.Source.NoteOwner = ctx.Source.NoteOwner ?? new NoteOwner();
                    this._breeds.Update(ctx.Source);

                    return ctx.Source.NoteOwner;
                }
            );
        }
    }
}
