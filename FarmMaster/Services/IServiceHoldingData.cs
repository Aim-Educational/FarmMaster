using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using Microsoft.EntityFrameworkCore;

namespace FarmMaster.Services
{
    public interface IServiceHoldingData : IServiceEntityData<Holding>
    {
        
    }

    public class ServiceHoldingData : IServiceHoldingData
    {
        readonly FarmMasterContext _context;

        public ServiceHoldingData(FarmMasterContext db)
        {
            this._context = db;
        }

        public int GetIdFor(Holding entity)
        {
            return entity.HoldingId;
        }

        public IQueryable<Holding> Query()
        {
            return this._context.Holdings;
        }

        public IQueryable<Holding> QueryAllIncluded()
        {
            return this._context.Holdings
                                .Include(h => h.OwnerContact)
                                .Include(h => h.Registrations)
                                 .ThenInclude(m => m.HoldingRegistration);
        }
    }
}
