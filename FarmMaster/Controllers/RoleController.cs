using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Filters;
using FarmMaster.Misc;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmMaster.Controllers
{
    [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.VIEW_ROLES })]
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

        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_ROLES })]
        public IActionResult Create([FromServices] FarmMasterContext db)
        {
            return View(new RoleCreateViewModel
            {
                AllKnownPermissions = db.EnumRolePermissions
            });
        }

        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_ROLES })]
        public IActionResult Edit(int id, 
                                  [FromQuery] string message, 
                                  [FromServices] FarmMasterContext db, 
                                  [FromServices] IServiceUserManager users)
        {
            var role = db.Roles.Include(r => r.Permissions)
                               .ThenInclude(p => p.EnumRolePermission)
                               .FirstOrDefault(r => r.RoleId == id);
            if(role == null)
                return NotFound();

            var user = users.UserFromCookieSession(HttpContext);
            if(!user.Role.CanModify(role))
                return this.RedirectToIndexWithMessage(ViewModelWithMessage.Type.Error, "You cannot modify a role that is higher in the hierarchy than your own.");
            if(user.Role.RoleId == role.RoleId)
                return this.RedirectToIndexWithMessage(ViewModelWithMessage.Type.Error, "You cannot modify your own role.");

            return View(new RoleEditViewModel
            {
                AllKnownPermissions = db.EnumRolePermissions,
                Role = role,
                Permissions = db.EnumRolePermissions
                                .ToDictionary(p => p.InternalName,
                                              p => role.Permissions.Any(m => m.EnumRolePermission == p))
            });
        }

        public IActionResult Assign([FromServices] FarmMasterContext db, [FromServices] IServiceUserManager users)
        {
            var myUser = users.UserFromCookieSession(HttpContext);
            return View(new RoleAssignViewModel
            {
                Users = db.Users.Include(u => u.Contact).Include(u => u.Role).Where(u => u != myUser),
                Roles = db.Roles
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_ROLES })]
        public IActionResult Create(RoleCreateViewModel model, 
                                    [FromServices] IServiceRoleManager roles,
                                    [FromServices] FarmMasterContext db, 
                                    [FromServices] IServiceUserManager users)
        {
            if(!ModelState.IsValid)
            {
                model.AllKnownPermissions = db.EnumRolePermissions;
                return View(model);
            }

            var user = users.UserFromCookieSession(HttpContext);
            if(user.Role.HierarchyOrder > model.HierarchyOrder)
            {
                model.MessageType = ViewModelWithMessage.Type.Error;
                model.Message = "You cannot create a role that is higher up in the hierarchy than your own.";
                model.AllKnownPermissions = db.EnumRolePermissions;
                return View(model);
            }

            var roleErrors = model.Permissions.Where(kvp =>
                !roles.HasPermission(user.Role, kvp.Key)
                && kvp.Value
            );
            if (roleErrors.Count() > 0)
            {
                var errorList = roleErrors.Select(r => r.Key)
                                          .Aggregate((s1, s2) => $"{s1}, {s2}");

                model.MessageType = ViewModelWithMessage.Type.Error;
                model.Message = $"You cannot create a role that has a permission your own role doesn't have. Pemissions = {errorList}";
                model.AllKnownPermissions = db.EnumRolePermissions;
                return View(model);
            }

            roles.Create(
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
        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_ROLES })]
        public IActionResult Edit(RoleEditViewModel model, 
                                  [FromServices] IServiceRoleManager roles,
                                  [FromServices] FarmMasterContext db, 
                                  [FromServices] IServiceUserManager users)
        {
            if (!ModelState.IsValid)
            {
                model.AllKnownPermissions = db.EnumRolePermissions;
                return View(model);
            }
            
            var user = users.UserFromCookieSession(HttpContext);
            if(!user.Role.CanModify(model.Role))
            {
                model.MessageType = ViewModelWithMessage.Type.Error;
                model.Message = "You cannot place a role higher up the hierarchy than your own.";
                model.AllKnownPermissions = db.EnumRolePermissions;
                return View(model);
            }

            if(user.Role.RoleId == model.Role.RoleId)
            {
                model.MessageType = ViewModelWithMessage.Type.Error;
                model.Message = "You cannot modify your own role.";
                model.AllKnownPermissions = db.EnumRolePermissions;
                return View(model);
            }
            
            db.Update(model.Role);
            foreach(var kvp in model.Permissions)
            {
                if(!roles.HasPermission(user.Role, kvp.Key) 
                && roles.HasPermission(model.Role, kvp.Key) != kvp.Value)
                {
                    model.MessageType = ViewModelWithMessage.Type.Error;
                    model.Message = $"You cannot modify permissions your role does not have. Permission = {kvp.Key}";
                    model.AllKnownPermissions = db.EnumRolePermissions;
                    return View(model);
                }

                if(kvp.Value)
                    roles.AddPermission(model.Role, kvp.Key, Misc.SaveChanges.No);
                else
                    roles.RemovePermission(model.Role, kvp.Key, Misc.SaveChanges.No);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //[HttpPost]
        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_ROLES })]
        public IActionResult Delete(int id, 
                                    [FromServices] IServiceRoleManager roles, 
                                    [FromServices] FarmMasterContext db,
                                    [FromServices] IServiceUserManager users)
        {
            try
            {
                var role = roles.FromIdAllIncluded(id);
                var user = users.UserFromCookieSession(HttpContext);
                if (!user.Role.CanModify(role))
                    return this.RedirectToIndexWithMessage(ViewModelWithMessage.Type.Error, "Cannot delete a role further up in the hierarchy than your own.");

                roles.RemoveRole(role);
                return this.RedirectToIndexWithMessage(ViewModelWithMessage.Type.Information, "Deleted role succesfully.");
            }
            catch(Exception ex)
            {
                return this.RedirectToIndexWithMessage(ViewModelWithMessage.Type.Error, ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [FarmAjaxReturnsMessage(EnumRolePermission.Names.ASSIGN_ROLES)]
        public IActionResult AjaxSetUserRole([FromBody]     AjaxSetUserRoleData data,
                                             [FromServices] FarmMasterContext db, 
                                             [FromServices] IServiceUserManager users, 
                                             [FromServices] IServiceRoleManager roles,
                                                            User myUser)
        {
            var toModifyUser = users.FromIdAllIncluded(data.userId);
            var role = roles.FromIdAllIncluded(data.roleId);
            if (toModifyUser == null)
                throw new Exception($"The user with id #{data.userId} does not exist.");
            if (role == null && data.roleId != int.MaxValue) // int.MaxValue is intentionally allowed to be null, so you can remove roles out right.
                throw new Exception($"The role with the id #{data.roleId} does not exist.");

            if (db.Entry(toModifyUser).State == EntityState.Detached)
                throw new Exception("Internal error. toModifyUser is not being tracked by EF");
            if (!myUser.Role.CanModify(toModifyUser.Role))
                throw new Exception("You cannot change the role of someone who's role is higher in the hierarchy than your own.");
            if (!myUser.Role.CanModify(role))
                throw new Exception($"You cannot assign the '{role.Name}' role as it is higher in the hierarchy than your own.");
            if (myUser == toModifyUser)
                throw new Exception("You cannot assign your own role, this is for security purposes, and to avoid accidental role lock outs.");

            toModifyUser.Role = role;
            db.SaveChanges();

            return new EmptyResult();
        }

        private IActionResult RedirectToIndexWithMessage(ViewModelWithMessage.Type type, string message)
        {
            return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        message = ViewModelWithMessage.CreateMessageQueryString(type, message)
                    }
                );
        }
    }

    public class AjaxSetUserRoleData : AjaxRequestModel
    {
       public int userId { get; set; }
       public int roleId { get; set; }
    }
}