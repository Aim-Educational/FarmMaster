using DataAccess;
using DataAccess.Constants;
using DataAccessLogic;
using FarmMaster.Module.Core.Controllers;
using FarmMaster.Module.Core.Models;
using Microsoft.Extensions.Logging;

namespace CrudModule.Controllers
{
    public class LocationController : CrudController<Location, ILocationManager>
    {
        // So we don't constantly recreate instances.
        static readonly CrudControllerConfig CONFIG = new CrudControllerConfig
        {
            DeletePolicy  = Permissions.Location.Delete,
            ManagePolicy  = Permissions.Location.ManageUI,
            ReadPolicy    = Permissions.Location.Read,
            WritePolicy   = Permissions.Location.Write,
            ViewSubFolder = "CrudModule/Location"
        };

        protected override CrudControllerConfig Config => CONFIG;

        public LocationController(
            ILocationManager locations,
            IUnitOfWork unitOfWork,
            ILogger<SpeciesController> logger
        )
        : base(locations, unitOfWork, logger)
        {
        }

        protected override Location CreateEntityFromModel(CrudCreateEditViewModel<Location> model)
        {
            var location = new Location
            {
                Type = model.Entity.Type // Type is only whitelisted during creation.
            };
            this.UpdateEntityFromModel(model, ref location);

            return location;
        }

        protected override void UpdateEntityFromModel(CrudCreateEditViewModel<Location> model, ref Location entity)
        {
            entity.Name      = model.Entity.Name;
            entity.HoldingId = model.Entity.HoldingId;
            
            // Whitelist changes.
            if(model.Entity.Holding != null && model.Entity.Type == LocationType.Holding)
            {
                var left  = entity.Holding ?? new LocationHolding();
                var right = model.Entity.Holding;

                left.Address       = right.Address;
                left.GridReference = right.GridReference;
                left.HoldingNumber = right.HoldingNumber;
                left.OwnerId       = right.OwnerId;
                left.Postcode      = right.Postcode;
                left.Timestamp     = right.Timestamp;

                entity.Holding = left;
            }
        }
    }
}