using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Constants;
using DataAccessLogic;
using FarmMaster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FarmMaster.Controllers
{
    public class SpeciesController : CrudController<Species, ISpeciesManager>
    {
        // So we don't constantly recreate instances.
        static readonly CrudControllerConfig CONFIG = new CrudControllerConfig 
        {
            DeletePolicy = Permissions.Species.Delete,
            ManagePolicy = Permissions.Species.ManageUI,
            ReadPolicy   = Permissions.Species.Read,
            WritePolicy  = Permissions.Species.Write
        };

        protected override CrudControllerConfig Config => CONFIG;

        public SpeciesController(
            ISpeciesManager species, 
            IUnitOfWork unitOfWork, 
            ILogger<SpeciesController> logger
        )
        : base(species, unitOfWork, logger)
        {
        }

        protected override Species CreateEntityFromModel(CrudCreateEditViewModel<Species> model)
        {
            var species = new Species();
            this.UpdateEntityFromModel(model, ref species);

            return species;
        }

        protected override void UpdateEntityFromModel(CrudCreateEditViewModel<Species> model, ref Species entity)
        {
            entity.Name             = model.Entity.Name;
            entity.GestrationPeriod = model.Entity.GestrationPeriod;
        }
    }
}