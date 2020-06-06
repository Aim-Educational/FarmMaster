using DataAccess;
using DataAccess.Constants;
using DataAccessLogic;
using FarmMaster.Module.Core.Controllers;
using FarmMaster.Module.Core.Models;
using Microsoft.Extensions.Logging;

namespace CrudModule.Controllers
{
    public class BreedController : CrudController<Breed, IBreedManager>
    {
        // So we don't constantly recreate instances.
        static readonly CrudControllerConfig CONFIG = new CrudControllerConfig
        {
            DeletePolicy = Permissions.Breed.Delete,
            ManagePolicy = Permissions.Breed.ManageUI,
            ReadPolicy = Permissions.Breed.Read,
            WritePolicy = Permissions.Breed.Write,
            ViewSubFolder = "CrudModule/Breed"
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
            entity.Name = model.Entity.Name;
            entity.SpeciesId = model.Entity.SpeciesId;
        }
    }
}