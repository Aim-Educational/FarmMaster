using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmMaster.Controllers
{
    public class RoleController : Controller
    {
        public IActionResult Index([FromServices] FarmMasterContext db)
        {
            return View(db.Roles);
        }

        public IActionResult Create([FromServices] FarmMasterContext db)
        {
            return View(new RoleCreateViewModel
            {
                AllKnownPermissions = db.EnumRolePermissions
            });
        }

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
                model.Permissions
                     .Where(kvp => kvp.Value)
                     .Select(kvp => kvp.Key)
                     .ToArray()
            );

            return Redirect("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
    }
}