using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccessLogic;
using FarmMaster.Services;
using FarmMaster.Constants;
using FarmMaster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DataAccess.Constants;

namespace FarmMaster.Controllers
{
    [Authorize(Policy = Policies.SeeAdminPanel)]
    public class AdminController : Controller
    {
        readonly UserManager<ApplicationUser> _users;
        readonly SignInManager<ApplicationUser> _signIn;
        readonly IAuthorizationService _auth;

        public AdminController(
            UserManager<ApplicationUser> users,
            SignInManager<ApplicationUser> signIn,
            IAuthorizationService auth
        )
        {
            this._users = users;
            this._signIn = signIn;
            this._auth = auth;
        }

        [Authorize(Policy = Permissions.Other.DebugUI)]
        public IActionResult ControlTest()
        {
            return View();
        }

        [Authorize(Policy = Permissions.Other.Settings)]
        public IActionResult Settings([FromServices] IOptionsSnapshot<EmailSenderConfig> emailConf)
        {
            return View(new AdminSettingsViewModel
            { 
                Email = emailConf.Value
            });
        }

        [Authorize(Policy = Permissions.Other.Settings)]
        public async Task<IActionResult> TestEmail([FromServices] IEmailSender email, [FromServices] UserManager<ApplicationUser> users)
        {
            var user    = await users.GetUserAsync(User);
            var address = await users.GetEmailAsync(user);

            await email.SendEmailAsync(address, "This is a test", "<h1>Lalafells are demons, change my mind</h1>");

            return RedirectToAction("Settings");
        }

        [Authorize(Policy = Permissions.User.ManageUI)]
        public IActionResult Users([FromServices] UserManager<ApplicationUser> users, [FromQuery] string error)
        {
            if(error != null)
                ModelState.AddModelError(string.Empty, error);

            return View(new AdminUsersViewModel
            {
                Users = users.Users
            });
        }

        [AllowAnonymous] // This action can be used by Admins, or if userId is the id of the currently logged in user
        public async Task<IActionResult> ManageUser([FromQuery] string userId)
        {
            var authResult = await this.CanManageUser(userId);
            if (!authResult.allowed) // Interesting tidbit, because IAuthorizationService fails the check, it forcefully takes the user to AccessDenied
                return RedirectToAction(authResult.selfManage ? "Login" : "Users", new { authResult.error });

            var user = authResult.user;
            return View(new AdminManageUserViewModel
            {
                Username = user.UserName,
                Email    = user.Email,
                Id       = Convert.ToString(user.Id)
            });
        }

        [HttpPost]
        [Authorize(Policy = Permissions.Other.Settings)]
        public IActionResult Email(AdminSettingsViewModel settings, [FromServices] IFarmMasterSettingsAccessor dbSettings)
        {
            if(!ModelState.IsValid)
                return RedirectToAction("Settings");

            var mutableSettings          = dbSettings.Settings;
            mutableSettings.SmtpServer   = settings.Email.Server;
            mutableSettings.SmtpPort     = settings.Email.Port;
            mutableSettings.SmtpUsername = settings.Email.Username;
            mutableSettings.SmtpPassword = settings.Email.Password;

            dbSettings.Settings = mutableSettings;
            return RedirectToAction("Settings");
        }

        [HttpPost]
        [AllowAnonymous] // See GET of this action
        public async Task<IActionResult> ManageUser(AdminManageUserViewModel model)
        {
            var authResult = await this.CanManageUser(model.Id);
            if(!authResult.allowed)
                return RedirectToAction(authResult.selfManage ? "Login" : "Users", new { authResult.error });

            if(ModelState.IsValid)
            {
                var results = new List<IdentityResult>();
                var user    = authResult.user;

                results.Add(await this._users.SetUserNameAsync(user, model.Username));

                if(model.Password != null || model.ConfirmPassword != null || model.CurrentPassword != null)
                {
                    if(model.Password != null && model.ConfirmPassword != null && model.CurrentPassword != null)
                    {
                        if(model.Password == model.ConfirmPassword)
                            results.Add(await this._users.ChangePasswordAsync(user, model.CurrentPassword, model.Password));
                        else
                            ModelState.AddModelError(nameof(model.ConfirmPassword), "New Password doesn't match Confirm Password.");
                    }
                    else
                        ModelState.AddModelError(nameof(model.Password), "All three password fields must be provided to change the password.");
                }

                foreach(var error in results.Where(r => !r.Succeeded).SelectMany(r => r.Errors))
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            model.CurrentPassword = null;
            model.Password        = null;
            model.ConfirmPassword = null;
            return View(model);
        }

        private async Task<(bool allowed, string error, ApplicationUser user, bool selfManage)> CanManageUser(string userId)
        {
            // Get our user, and the one we're modifying.
            var loggedInUser = await this._users.GetUserAsync(User);
            if(loggedInUser == null)
                return (false, "You are not logged in", null, false);

            var user = await this._users.FindByIdAsync(userId);
            if(user == null)
                return (false, "User does not exist", null, false);

            // Get our user principal so we can check if we're an admin
            var principal = await this._signIn.CreateUserPrincipalAsync(loggedInUser);
            var isAdmin   = await this._auth.AuthorizeAsync(principal, Policies.IsAdmin);
            if(!isAdmin.Succeeded)
            {
                if(user.Id == loggedInUser.Id)
                    return (true, null, user, true); // ALLOW: We're editing ourself.

                return (false, "You do not have permission to edit other users.", user, false);
            }

            return (true, null, user, false); // ALLOW: We're an admin.
        }
    }
}