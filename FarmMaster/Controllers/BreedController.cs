using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Constants;
using DataAccessLogic;
using FarmMaster.Models;
using FarmMaster.Module.Core.Controllers;
using FarmMaster.Module.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FarmMaster.Controllers
{
    public class BreedController : CrudController<Breed, IBreedManager>
    {
        // So we don't constantly recreate instances.
        static readonly CrudControllerConfig CONFIG = new CrudControllerConfig 
        {
            DeletePolicy = Permissions.Breed.Delete,
            ManagePolicy = Permissions.Breed.ManageUI,
            ReadPolicy   = Permissions.Breed.Read,
            WritePolicy  = Permissions.Breed.Write
        };

        protected override CrudControllerConfig Config => CONFIG;

        public BreedController(
            IBreedManager breeds, 
            IUnitOfWork unitOfWork, 
            ILogger<SpeciesController> logger
        )
        : base(breeds, unitOfWork, logger)
        {
        }

        protected override Breed CreateEntityFromModel(CrudCreateEditViewModel<Breed> model)
        {
            var breed = new Breed();
            this.UpdateEntityFromModel(model, ref breed);

            return breed;
        }

        protected override void UpdateEntityFromModel(CrudCreateEditViewModel<Breed> model, ref Breed entity)
        {
            entity.Name             = model.Entity.Name;
            entity.SpeciesId        = model.Entity.SpeciesId;
        }
    }
}