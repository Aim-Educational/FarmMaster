using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Filters;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmMaster.Controllers
{
    [FarmAuthorise(PermsAND: new[] { EnumRolePermissionNames.VIEW_ROLES })]
    public class RoleController : Controller
    {
        public IActionResult Index([FromQuery] string message, [FromServices] FarmMasterContext db)
        {
            var model = new RoleIndexViewModel
            {
                Roles = db.Roles.Include(r => r.Permissions)
            };
            model.ParseMessageQueryString(message);

            return View(model);
        }

        [FarmAuthorise(PermsAND: new[] { EnumRolePermissionNames.EDIT_ROLES })]
        public IActionResult Create([FromServices] FarmMasterContext db)
        {
            return View(new RoleCreateViewModel
            {
                AllKnownPermissions = db.EnumRolePermissions
            });
        }

        [FarmAuthorise(PermsAND: new[] { EnumRolePermissionNames.EDIT_ROLES })]
        public IActionResult Edit(int id, [FromServices] FarmMasterContext db)
        {
            var role = db.Roles.Include(r => r.Permissions)
                               .ThenInclude(p => p.EnumRolePermission)
                               .FirstOrDefault(r => r.RoleId == id);
            if(role == null)
                return NotFound();

            return View(new RoleEditViewModel
            {
                AllKnownPermissions = db.EnumRolePermissions,
                Role = role,
                Permissions = db.EnumRolePermissions
                                .ToDictionary(p => p.InternalName,
                                              p => role.Permissions.Any(m => m.EnumRolePermission == p))
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { EnumRolePermissionNames.EDIT_ROLES })]
        public IActionResult Create(RoleCreateViewModel model, [FromServices] IServiceRoleManager roles,
                                    [FromServices] FarmMasterContext db)
        {
            if(!ModelState.IsValid)
            {
                model.AllKnownPermissions = db.EnumRolePermissions;
                return View(model);
            }

            roles.CreateRole(
                model.Name,
                model.Description,
                model.Permissions
                     .Where(kvp => kvp.Value)
                     .Select(kvp => kvp.Key)
                     .ToArray()
            );

            return Redirect("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { EnumRolePermissionNames.EDIT_ROLES })]
        public IActionResult Edit(RoleEditViewModel model, [FromServices] IServiceRoleManager roles,
                                  [FromServices] FarmMasterContext db)
        {
            if (!ModelState.IsValid)
            {
                model.AllKnownPermissions = db.EnumRolePermissions;
                return View(model);
            }

            db.Update(model.Role);
            foreach(var kvp in model.Permissions)
            {
                if(kvp.Value)
                    roles.AddPermission(model.Role, kvp.Key, Misc.SaveChanges.No);
                else
                    roles.RemovePermission(model.Role, kvp.Key, Misc.SaveChanges.No);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //[HttpPost]
        [FarmAuthorise(PermsAND: new[] { EnumRolePermissionNames.EDIT_ROLES })]
        public IActionResult Delete(int id, [FromServices] IServiceRoleManager roles, [FromServices] FarmMasterContext db)
        {
            try
            {
                var role = roles.RoleFromId(id);
                roles.RemoveRole(role);

                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        message = ViewModelWithMessage.CreateMessageQueryString(ViewModelWithMessage.Type.Information, "Deleted role succesfully.")
                    }
                );
            }
            catch(Exception ex)
            {
                return RedirectToAction(
                    nameof(Index), 
                    new
                    {
                        message = ViewModelWithMessage.CreateMessageQueryString(ViewModelWithMessage.Type.Error, ex.Message)
                    }
                );
            }
        }
    }
}