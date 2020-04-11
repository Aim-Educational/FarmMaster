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
    public class CrudControllerConfig
    {
        public string WritePolicy { get; set; }
        public string ReadPolicy { get; set; }
        public string DeletePolicy { get; set; }
        public string ManagePolicy { get; set; }
    }

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

        protected abstract EntityT CreateEntityFromModel(CrudCreateEditViewModel<EntityT> model);
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
            var isAuthed = await auth.AuthorizeAsync(User, this.Config.ManagePolicy);
            if (!isAuthed.Succeeded)
                return RedirectToAction("Account/AccessDenied");

            return View("CreateEdit", new CrudCreateEditViewModel<EntityT>
            {
                IsCreate = true
            });
        }

        public virtual async Task<IActionResult> Edit(int? id, [FromServices] IAuthorizationService auth)
        {
            var isAuthed = await auth.AuthorizeAsync(User, this.Config.ManagePolicy);
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
            var isAuthed = await auth.AuthorizeAsync(User, this.Config.ManagePolicy);
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
            var isAuthed = await auth.AuthorizeAsync(User, this.Config.ManagePolicy);
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
            var isAuthed = await auth.AuthorizeAsync(User, this.Config.ManagePolicy);
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
