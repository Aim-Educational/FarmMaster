using DataAccess;
using DataAccessLogic.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLogic
{
    public interface ILocationManager : ICrudAsync<Location>
    {
    }

    public class LocationManager : DbContextCrud<Location, FarmMasterContext>, ILocationManager
    {
        static MutuallyExclusiveForeignKeys<LocationType, Location> _keysInstance;

        // This is only public so that the test cases can easily access this.
        public static MutuallyExclusiveForeignKeys<LocationType, Location> _MutualKeys
        {
            get
            {
                if(_keysInstance != null)
                    return _keysInstance;

                _keysInstance = new MutuallyExclusiveForeignKeys<LocationType, Location>()
                    .Define(LocationType.Unknown, _ => null,        _ => null)
                    .Define(LocationType.Holding, l => l.HoldingId, l => l.Holding);
                
                return _keysInstance;
            }
        }

        public LocationManager(FarmMasterContext db) : base(db)
        {
        }

        public override IQueryable<Location> IncludeAll(IQueryable<Location> query)
        {
            return query.Include(c => c.NoteOwner)
                         .ThenInclude(o => o.NoteEntries)
                        .Include(c => c.Holding)
                         .ThenInclude(h => h.Owner);
        }

        public override ResultObject PreCreateCheck(Location entity)
        {
            return this.PreUpdateCheck(entity);
        }

        public override ResultObject PreUpdateCheck(Location entity)
        {
            return _MutualKeys.IsUniquelySet(entity.Type, entity)
                ? null
                : new ResultObject 
                { 
                    Succeeded = false, 
                    Errors = new[]
                    { 
                        $"Location {entity.Name} is of type {entity.Type} yet it has more than 1 location type id set."
                    } 
                };
        }
    }
}
