using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DataAccessLogic
{
    public interface ISpeciesManager : ICrudAsync<Species>
    {
    }

    public class SpeciesManager : DbContextCrud<Species, FarmMasterContext>, ISpeciesManager
    {
        public SpeciesManager(FarmMasterContext db) : base(db)
        {

        }

        public override IQueryable<Species> IncludeAll(IQueryable<Species> query)
        {
            return query.Include(s => s.NoteOwner)
                        .ThenInclude(o => o.NoteEntries);
        }
    }
}
