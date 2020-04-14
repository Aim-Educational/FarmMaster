using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    /// <summary>
    /// Configuration passed directly by an implementor of <see cref="CrudController{EntityT, CrudT}"/>.
    /// </summary>
    public class CrudControllerConfig
    {
        /// <summary>
        /// The policy that the user must pass to be able to create or edit the entity.
        /// </summary>
        public string WritePolicy { get; set; }


        /// <summary>
        /// The policy that the user must pass to be able to view the entity.
        /// </summary>
        public string ReadPolicy { get; set; }


        /// <summary>
        /// The policy that the user must pass to be able to delete the entity.
        /// </summary>
        public string DeletePolicy { get; set; }


        /// <summary>
        /// The policy that the user must pass to be able to see the list page for the entity.
        /// </summary>
        public string ManagePolicy { get; set; }
    }

    /// <summary>
    /// The base class for controllers that implement basic CRUD.
    /// </summary>
    /// <remarks>
    /// This class provides the following routes:
    /// 
    ///     <list type="bullet">
    ///         <item>Index  - Shows a table to view all of the entities. Requires <see cref="CrudControllerConfig.ManagePolicy"/></item>
    ///         <item>Create - Allows creation of the entity. Requires <see cref="CrudControllerConfig.WritePolicy"/></item>
    ///         <item>Edit   - Allows viewing and optionally editing of an entity. Requires <see cref="CrudControllerConfig.ReadPolicy"/> or WritePolicy</item>
    ///         <item>Delete - Allows deletion of an entity. Requires <see cref="CrudControllerConfig.DeletePolicy"/></item>
    ///     </list>
    ///     
    /// To implement this class, do the following:
    /// 
    ///     <list type="bullet">
    ///         <item>
    ///             Implement <see cref="CreateEntityFromModel(CrudCreateEditViewModel{EntityT})"/>
    ///             and <see cref="UpdateEntityFromModel(CrudCreateEditViewModel{EntityT}, ref EntityT)"/>
    ///         </item>
    ///         <item>
    ///             Create a view called CreateEdit which uses the _LayoutGenericCrud layout and
    ///             <see cref="CrudCreateEditViewModel{EntityT}"/> as the model.
    ///         </item>
    ///         <item>
    ///             Create a view called Index which uses <see cref="CrudIndexViewModel{EntityT}"/> as the model.
    ///         </item>
    ///         <item>
    ///             ??? Profit.
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <typeparam name="EntityT">The type of entity that to provide CRUD for.</typeparam>
    /// <typeparam name="CrudT">The <see cref="ICrudAsync{EntityT}"/> type to be injected with. This is what allows this class to perform CRUD.</typeparam>
    [Authorize]
    public abstract class CrudController<EntityT, CrudT> : Controller
    where CrudT : ICrudAsync<EntityT>
    where EntityT : class
    {
        protected readonly CrudT       Crud;
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly ILogger     Logger;

        protected abstract CrudControllerConfig Config { get; }

        public CrudController(
            CrudT       crud,
            IUnitOfWork unitOfWork,
            ILogger     logger
        )
        {
            this.Crud       = crud;
            this.UnitOfWork = unitOfWork;
            this.Logger     = logger;
        }

        /// <summary>
        /// Creates an instance of <typeparamref name="EntityT"/> from the given <paramref name="model"/>
        /// </summary>
        /// <param name="model">The model to create the entity from.</param>
        /// <returns>The <typeparamref name="EntityT"/> create from <paramref name="model"/>.</returns>
        protected abstract EntityT CreateEntityFromModel(CrudCreateEditViewModel<EntityT> model);

        /// <summary>
        /// Updates an instance of <typeparamref name="EntityT"/> from the given <paramref name="model"/>
        /// </summary>
        /// <param name="model">The model to update the entity from.</param>
        /// <param name="entity">The entity to update.</param>
        protected abstract void UpdateEntityFromModel(CrudCreateEditViewModel<EntityT> model, ref EntityT entity);

        public virtual async Task<IActionResult> Index([FromServices] IAuthorizationService auth)
        {
            var isAuthed = await auth.AuthorizeAsync(User, this.Config.ManagePolicy);
            if(!isAuthed.Succeeded)
                return RedirectToAction("AccessDenied", "Account");

            return View("Index", new CrudIndexViewModel<EntityT>
            {
                Entities = this.Crud.Query() // Effectively a .GetAll
            });
        }

        public virtual async Task<IActionResult> Create([FromServices] IAuthorizationService auth)
        {
            var isAuthed = await auth.AuthorizeAsync(User, this.Config.WritePolicy);
            if (!isAuthed.Succeeded)
                return RedirectToAction("Account/AccessDenied");

            return View("CreateEdit", new CrudCreateEditViewModel<EntityT>
            {
                IsCreate = true
            });
        }

        public virtual async Task<IActionResult> Edit(int? id, [FromServices] IAuthorizationService auth)
        {
            var isAuthed = await auth.AuthorizeAsync(User, this.Config.ReadPolicy); // Allow viewing, but default is hidden behind WritePolicy.
            if (!isAuthed.Succeeded)
                return RedirectToAction("AccessDenied", "Account");

            var result = await this.Crud.GetByIdAsync(id ?? -1);
            if (!result.Succeeded)
                return RedirectToAction("Index", new { error = result.GatherErrorMessages().FirstOrDefault() });

            return View("CreateEdit", new CrudCreateEditViewModel<EntityT>
            {
                Entity   = result.Value,
                IsCreate = false
            });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(
            CrudCreateEditViewModel<EntityT>     model, 
            [FromServices] IAuthorizationService auth
        )
        {
            var isAuthed = await auth.AuthorizeAsync(User, this.Config.WritePolicy);
            if (!isAuthed.Succeeded)
                return Forbid();

            if (ModelState.IsValid)
            {
                var entity = this.CreateEntityFromModel(model);

                using (var workScope = this.UnitOfWork.Begin("Create Entity"))
                {
                    var result = await this.Crud.CreateAsync(entity);
                    if (!result.Succeeded)
                    {
                        workScope.Rollback("CreateAsync failed.");

                        foreach (var error in result.GatherErrorMessages())
                            ModelState.AddModelError(string.Empty, error);
                        return View("CreateEdit", model);
                    }

                    entity = result.Value;
                    workScope.Commit();
                }

                this.Logger.LogInformation(
                    "Entity {Entity} created by {User}",
                    typeof(EntityT).Name,
                    User.FindFirstValue(ClaimTypes.Name)
                );

                return RedirectToAction("Edit", new { id = this.GetEntityId(entity) });
            }

            return View("CreateEdit", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit(
            CrudCreateEditViewModel<EntityT>     model,
            [FromServices] IAuthorizationService auth
        )
        {
            var isAuthed = await auth.AuthorizeAsync(User, this.Config.WritePolicy);
            if (!isAuthed.Succeeded)
                return Forbid();

            if (ModelState.IsValid)
            {
                var result = await this.Crud.GetByIdAsync(this.GetEntityId(model.Entity));
                if (!result.Succeeded)
                    return RedirectToAction("Index", new { error = result.GatherErrorMessages().FirstOrDefault() });

                var dbEntity = result.Value;
                this.UpdateEntityFromModel(model, ref dbEntity);

                using (var workScope = this.UnitOfWork.Begin("Edit Entity"))
                {
                    var updateResult = this.Crud.Update(dbEntity);
                    if (!updateResult.Succeeded)
                    {
                        workScope.Rollback("CreateAsync failed.");

                        foreach (var error in result.GatherErrorMessages())
                            ModelState.AddModelError(string.Empty, error);
                        return View("CreateEdit", model);
                    }

                    workScope.Commit();
                }

                this.Logger.LogInformation(
                    "Entity {Entity} updated by {User}",
                    typeof(EntityT).Name,
                    User.FindFirstValue(ClaimTypes.Name)
                );
            }

            return View("CreateEdit", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int? id, [FromServices] IAuthorizationService auth)
        {
            var isAuthed = await auth.AuthorizeAsync(User, this.Config.DeletePolicy);
            if (!isAuthed.Succeeded)
                return Forbid();

            var result = await this.Crud.GetByIdAsync(id ?? -1);
            if (!result.Succeeded)
                return RedirectToAction("Edit", new { error = result.GatherErrorMessages().FirstOrDefault() });

            using (var workScope = this.UnitOfWork.Begin("Delete Entity"))
            {
                var deleteResult = this.Crud.Delete(result.Value);
                if (!deleteResult.Succeeded)
                {
                    workScope.Rollback("Delete failed.");

                    return RedirectToAction("Edit", new { error = deleteResult.Errors.First() });
                }

                workScope.Commit();
            }

            this.Logger.LogInformation(
                "Entity {Entity} deleted by {User}",
                typeof(EntityT).Name,
                User.FindFirstValue(ClaimTypes.Name)
            );

            return RedirectToAction("Index");
        }

        private int GetEntityId(EntityT entity)
        {
            var keyProp = typeof(EntityT).GetProperties()
                                         .Single(p => p.IsDefined(typeof(KeyAttribute), true));

            return (int)keyProp.GetValue(entity);
        }
    }
}
