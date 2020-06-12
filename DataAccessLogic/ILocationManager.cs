using DataAccess;
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
        /**
         * To make it easier to automate the mutual exclusivity checks:
         *      - When we're performing the check on a location, we load all of its mutually exclusive ID fields into ID_HOLDER
         *      - The index for each ID corresponds to their LocationType, so the ID for a Holding would be index 1 for example.
         *      - This essentially allows us to lookup that our desired type has an ID, while ensuring that other types don't have one.
         * */
        static readonly int      LOCATION_TYPE_COUNT        = Enum.GetNames(typeof(LocationType)).Length;
        static readonly int?[]   LOCATION_ID_HOLDER         = new int?[LOCATION_TYPE_COUNT];
        static readonly object[] LOCATION_INSTANCE_HOLDER   = new object[LOCATION_TYPE_COUNT];

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
            this.PopulateHolders(entity);
            var isMutuallySet = this.CheckTypeIsMutuallySet(entity.Type);

            return isMutuallySet
                ? null
                : new ResultObject 
                { 
                    Succeeded = false, 
                    Errors = new[]
                    { 
                        $"Location {entity.Name} is of type {entity.Type} yet it has more than 1 location type id set.",
                        $"Location Ids: {LOCATION_ID_HOLDER}"
                    } 
                };
        }

        private void PopulateHolders(Location entity)
        {
            LOCATION_ID_HOLDER[(int)LocationType.Unknown] = null;
            LOCATION_ID_HOLDER[(int)LocationType.Holding] = entity.HoldingId;

            LOCATION_INSTANCE_HOLDER[(int)LocationType.Unknown] = null;
            LOCATION_INSTANCE_HOLDER[(int)LocationType.Holding] = entity.Holding;
        }

        private bool CheckTypeIsMutuallySet(LocationType type)
        {
            var amISet = false;

            for(int i = 0; i < LOCATION_TYPE_COUNT; i++)
            {
                var isSet = LOCATION_ID_HOLDER[i] != null || LOCATION_INSTANCE_HOLDER[i] != null;
                if (i == (int)type && isSet)
                    amISet = true;
                else if(isSet)
                    return false;
            }

            return amISet;
        }
    }
}
