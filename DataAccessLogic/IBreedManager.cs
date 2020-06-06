using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DataAccessLogic
{
    public interface IBreedManager : ICrudAsync<Breed>
    {
    }

    public class BreedManager : DbContextCrud<Breed, FarmMasterContext>, IBreedManager
    {
        public BreedManager(FarmMasterContext db) : base(db)
        {

        }

        public override IQueryable<Breed> IncludeAll(IQueryable<Breed> query)
        {
            return query.Include(b => b.NoteOwner)
                         .ThenInclude(o => o.NoteEntries)
                        .Include(b => b.Species)
                         .ThenInclude(s => s.NoteOwner)
                          .ThenInclude(o => o.NoteEntries);
        }
    }
}
