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
        Holding Create(string name, string holdingNumber, string gridReference, string address, string postCode, Contact owner);
        bool AddRegistrationByName(Holding holding, string regInternalName);
        bool RemoveRegistrationByName(Holding holding, string regInternalName);
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

        public Holding Create(string name, string holdingNumber, string gridReference, string address, string postCode, Contact owner)
        {
            if(owner == null)
                throw new ArgumentNullException("owner");

            var holding = new Holding
            {
                Name = name,
                HoldingNumber = holdingNumber,
                GridReference = gridReference,
                Address = address,
                Postcode = postCode,
                OwnerContact = owner
            };

            this._context.Add(holding);
            this._context.SaveChanges();

            this._context.Entry(holding).Collection(h => h.Registrations).Load();
            return holding;
        }

        public bool AddRegistrationByName(Holding holding, string regInternalName)
        {
            if(holding == null)
                throw new ArgumentNullException("holding");

            var reg = this.GetRegistrationByName(regInternalName);
            if(holding.Registrations.Any(r => r.HoldingRegistrationId == reg.EnumHoldingRegistrationId))
                return false;

            var map = new MapHoldingRegistrationToHolding
            {
                Holding = holding,
                HoldingRegistration = reg                
            };

            this._context.Add(map);
            this._context.SaveChanges();

            return true;
        }

        public bool RemoveRegistrationByName(Holding holding, string regInternalName)
        {
            if (holding == null)
                throw new ArgumentNullException("holding");

            var reg = this.GetRegistrationByName(regInternalName);
            var map = holding.Registrations.FirstOrDefault(m => m.HoldingRegistrationId == reg.EnumHoldingRegistrationId);
            if(map == null)
                return false;

            this._context.Remove(map);
            this._context.SaveChanges();

            return true;
        }

        private EnumHoldingRegistration GetRegistrationByName(string regInternalName)
        {
            var reg = this._context.EnumHoldingRegistrations.FirstOrDefault(r => r.InternalName == regInternalName);
            if (reg == null)
                throw new ArgumentOutOfRangeException($"There is no registration with the internal name of '{regInternalName}'");

            return reg;
        }
    }
}
