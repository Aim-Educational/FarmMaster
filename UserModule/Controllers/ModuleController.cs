﻿using AccountModule.Constants;
using DataAccess;
using DataAccess.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserModule.Models;

namespace UserModule.Controllers
{
    [Area("Admin")]
    [Route("/Admin/{action}")]
    public class ModuleController : Controller
    {
        // This allows me to simplify folder structure and routing, at the cost of having to remember to use these strings.
        const string VIEW_USERS = "~/Views/UserModule/Users.cshtml";
        const string VIEW_MANAGE_USER = "~/Views/UserModule/ManageUser.cshtml";

        readonly UserManager<ApplicationUser> _users;
        readonly SignInManager<ApplicationUser> _signIn;
        readonly IAuthorizationService _auth;

        public ModuleController(
            UserManager<ApplicationUser> users,
            SignInManager<ApplicationUser> signIn,
            IAuthorizationService auth
        )
        {
            this._users = users;
            this._signIn = signIn;
            this._auth = auth;
        }

        [Authorize(Policy = Permissions.User.ManageUI)]
        public IActionResult Users([FromServices] UserManager<ApplicationUser> users, [FromQuery] string error)
        {
            if (error != null)
                this.ModelState.AddModelError(string.Empty, error);

            return this.View(VIEW_USERS, new AdminUsersViewModel
            {
                Users = users.Users
            });
        }

        [AllowAnonymous] // This action can be used by Admins, or if userId is the id of the currently logged in user
        public async Task<IActionResult> ManageUser([FromQuery] string userId)
        {
            var authResult = await this.CanManageUser(userId);
            if (!authResult.allowed) // Interesting tidbit, because IAuthorizationService fails the check, it forcefully takes the user to AccessDenied
                return this.RedirectToAction(authResult.selfManage ? "Login" : "Users", new { authResult.error });

            var user = authResult.user;
            return this.View(VIEW_MANAGE_USER, new AdminManageUserViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                Id = Convert.ToString(user.Id),
                ShowDeleteButton = authResult.hasDeletePerm
            });
        }

        [AllowAnonymous] // See GET of ManageUser
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (userId == null)
                return this.RedirectToAction("Users", new { error = "No user id was specified." });

            var authResult = await this.CanManageUser(userId);
            if (!authResult.allowed)
                return this.RedirectToAction(authResult.selfManage ? "Login" : "Users", new { authResult.error });

            if (!authResult.hasDeletePerm)
                return this.RedirectToAction(authResult.selfManage ? "Login" : "Users", new { error = "You do not have permission to delete this user." });

            if (authResult.isAdmin)
                return this.RedirectToAction("Users", new { error = "Admins cannot be deleted." });

            var deletingSelf = (await this._users.GetUserAsync(this.User)) == authResult.user;
            if (deletingSelf)
                await this._signIn.SignOutAsync();

            var result = await this._users.DeleteAsync(authResult.user);
            if (!result.Succeeded)
                return this.RedirectToAction(authResult.selfManage ? "Login" : "Users", new { error = result.Errors.First().Description });

            return this.RedirectToAction(deletingSelf ? "Login" : "Users", new { error = "User deleted successfully!" });
        }

        [HttpPost]
        [AllowAnonymous] // See GET of this action
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUser(AdminManageUserViewModel model)
        {
            var authResult = await this.CanManageUser(model.Id);
            if (!authResult.allowed)
                return this.RedirectToAction(authResult.selfManage ? "Login" : "Users", new { authResult.error });

            if (this.ModelState.IsValid)
            {
                var results = new List<IdentityResult>();
                var user = authResult.user;

                results.Add(await this._users.SetUserNameAsync(user, model.Username));

                if (model.Password != null || model.ConfirmPassword != null || model.CurrentPassword != null)
                {
                    if (model.Password != null && model.ConfirmPassword != null && model.CurrentPassword != null)
                    {
                        if (model.Password == model.ConfirmPassword)
                            results.Add(await this._users.ChangePasswordAsync(user, model.CurrentPassword, model.Password));
                        else
                            this.ModelState.AddModelError(nameof(model.ConfirmPassword), "New Password doesn't match Confirm Password.");
                    }
                    else
                        this.ModelState.AddModelError(nameof(model.Password), "All three password fields must be provided to change the password.");
                }

                foreach (var error in results.Where(r => !r.Succeeded).SelectMany(r => r.Errors))
                    this.ModelState.AddModelError(string.Empty, error.Description);
            }

            model.CurrentPassword = null;
            model.Password = null;
            model.ConfirmPassword = null;
            return this.View(VIEW_MANAGE_USER, model);
        }

        private async Task<(bool allowed, string error, ApplicationUser user, bool selfManage, bool hasDeletePerm, bool isAdmin)>
        CanManageUser(string userId)
        {
            // Get our user, and the one we're modifying.
            var loggedInUser = await this._users.GetUserAsync(this.User);
            if (loggedInUser == null)
                return (false, "You are not logged in", null, false, false, false);

            var user = await this._users.FindByIdAsync(userId);
            if (user == null)
                return (false, "User does not exist", null, false, false, false);

            // Get our user principal so we can check if we're an admin
            var principal = await this._signIn.CreateUserPrincipalAsync(loggedInUser);
            var isAdmin = await this._auth.AuthorizeAsync(principal, Policies.IsAdmin);
            var canDeleteUsers = await this._auth.AuthorizeAsync(principal, Permissions.User.Delete);
            if (!isAdmin.Succeeded)
            {
                if (user.Id == loggedInUser.Id)
                    return (true, null, user, true, true, false); // ALLOW: We're editing ourself.

                // If we're not an admin, and if we're not managing ourself, check if we can at least read users.
                var canReadUsers = await this._auth.AuthorizeAsync(principal, Permissions.User.Read);
                if (canReadUsers.Succeeded)
                    return (true, null, user, false, canDeleteUsers.Succeeded, false);

                return (false, "You do not have permission to edit other users.", user, false, false, false);
            }

            return (true, null, user, false, true, true); // ALLOW: We're an admin.
        }
    }
}
