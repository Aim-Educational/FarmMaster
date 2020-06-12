using DataAccess;
using DataAccess.Constants;
using DataAccessGraphQL;
using DataAccessGraphQL.Util;
using DataAccessLogic;
using GraphQL.Types;

namespace CrudModule.GraphQL.Mutations
{
    public class LocationRootMutation : ObjectGraphType<Location>
    {
        readonly DataAccessUserContext _context;
        readonly ILocationManager _locations;
        readonly INoteManager _notes;
        readonly IUnitOfWork _unitOfWork;

        public LocationRootMutation(
            GraphQLUserContextAccessor accessor,
            ILocationManager locations,
            INoteManager notes,
            IUnitOfWork unitOfWork
        )
        {
            this._context = accessor.Context;
            this._locations = locations;
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
                Permissions.Location.WriteNotes,
                ctx =>
                {
                    ctx.Source.NoteOwner = ctx.Source.NoteOwner ?? new NoteOwner();
                    this._locations.Update(ctx.Source);

                    return ctx.Source.NoteOwner;
                }
            );
        }
    }
}
