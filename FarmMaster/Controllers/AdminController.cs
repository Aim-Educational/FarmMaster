using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccessLogic;
using FarmMaster.Services;
using FarmMaster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DataAccess.Constants;
using EmailSender;
using Microsoft.Extensions.DependencyInjection;
using AccountModule.Constants;

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
        public IActionResult Settings([FromServices] IOptionsSnapshot<EmailSenderConfig> emailConf, [FromQuery] string emailTestError)
        {
            return View(new AdminSettingsViewModel
            {
                EmailError = emailTestError,
                Email = emailConf.Value
            });
        }

        #pragma warning disable MVC1004 // idk, It doesn't like it when I call parameters "users".
        [Authorize(Policy = Permissions.Other.Settings)]
        public async Task<IActionResult> TestEmail(
            [FromServices] ITemplatedEmailSender email, 
            [FromServices] UserManager<ApplicationUser> users
        )
        {
            var user    = await users.GetUserAsync(User);
            var address = await users.GetEmailAsync(user);

            // This is so I can also test templates, as well as letting the user test their settings.
            var template = new EmailTemplate("<h1>{{ race }} are demons, change my mind. There's a link {{ @a#b?c=race }}</h1>");
            var values   = new EmailTemplateValues() { { "race", "Lalafells" } };
            var result   = await email.SendTemplatedEmailAsync(address, "This is a test email", template, values);

            if(!result.Succeeded)
                return RedirectToAction("Settings", new { emailTestError = result.Error });

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
                Username         = user.UserName,
                Email            = user.Email,
                Id               = Convert.ToString(user.Id),
                ShowDeleteButton = authResult.hasDeletePerm
            });
        }

        [AllowAnonymous] // See GET of ManageUser
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (userId == null)
                return RedirectToAction("Users", new { error = "No user id was specified." });

            var authResult = await this.CanManageUser(userId);
            if (!authResult.allowed)
                return RedirectToAction(authResult.selfManage ? "Login" : "Users", new { authResult.error });

            if (!authResult.hasDeletePerm)
                return RedirectToAction(authResult.selfManage ? "Login" : "Users", new { error = "You do not have permission to delete this user." });

            if(authResult.isAdmin)
                return RedirectToAction("Users", new { error = "Admins cannot be deleted." });

            var deletingSelf = (await this._users.GetUserAsync(User)) == authResult.user;
            if (deletingSelf)
                await this._signIn.SignOutAsync();

            var result = await this._users.DeleteAsync(authResult.user);
            if (!result.Succeeded)
                return RedirectToAction(authResult.selfManage ? "Login" : "Users", new { error = result.Errors.First().Description });

            return RedirectToAction(deletingSelf ? "Login" : "Users", new { error = "User deleted successfully!" });
        }

        [HttpPost]
        [Authorize(Policy = Permissions.Other.Settings)]
        [ValidateAntiForgeryToken]
        public IActionResult Email(
            AdminSettingsViewModel settings, 
            [FromServices] IFarmMasterSettingsAccessor dbSettings,
            [FromServices] ITemplatedEmailSender email
        )
        {
            if(!ModelState.IsValid)
                return RedirectToAction("Settings");

            var mutableSettings          = dbSettings.Settings;
            mutableSettings.SmtpServer   = settings.Email.Smtp.Server;
            mutableSettings.SmtpPort     = settings.Email.Smtp.Port;
            mutableSettings.SmtpUsername = settings.Email.Smtp.Username;
            mutableSettings.SmtpPassword = settings.Email.Smtp.Password;

            dbSettings.Settings = mutableSettings;

            /**
             * Just so the flow of email is documented:
             * 
             *  - ConfigureEmailOptions will update IOptions, IOptionsSnapshot, and IOptionsMonitor with values
             *    from the database.
             *    
             *  - ITemplatedEmailSender doesn't recieve the new values automatically because ASP and its conflicting/open-ended documentation,
             *    so we need a way to notify it of changes to the config.
             *    
             *  - Since this is the only place where the config can be changed, after updating the database with new settings (code above),
             *    we construct an IOptionSnapshot for the *first time for this request*, causing it to contain up-to-date info via ConfigureEmailOptions.
             *    
             *  - After that, we call ReloadAsync on the ITemplatedEmailSender so it can update its connection.
             * */

            // This will cause all IConfigureOptions to be ran on the new values.
            var upToDateConfig = HttpContext.RequestServices.GetRequiredService<IOptionsSnapshot<EmailSenderConfig>>().Value;

            try
            { 
                email.ReloadAsync(upToDateConfig).Wait();
            }
            catch(Exception ex)
            {
                settings.EmailError = ex.Message;
                return View("Settings", settings);
            }

            return RedirectToAction("Settings");
        }

        [HttpPost]
        [AllowAnonymous] // See GET of this action
        [ValidateAntiForgeryToken]
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

        private async Task<(bool allowed, string error, ApplicationUser user, bool selfManage, bool hasDeletePerm, bool isAdmin)> 
        CanManageUser(string userId)
        {
            // Get our user, and the one we're modifying.
            var loggedInUser = await this._users.GetUserAsync(User);
            if(loggedInUser == null)
                return (false, "You are not logged in", null, false, false, false);

            var user = await this._users.FindByIdAsync(userId);
            if(user == null)
                return (false, "User does not exist", null, false, false, false);

            // Get our user principal so we can check if we're an admin
            var principal       = await this._signIn.CreateUserPrincipalAsync(loggedInUser);
            var isAdmin         = await this._auth.AuthorizeAsync(principal, Policies.IsAdmin);
            var canDeleteUsers  = await this._auth.AuthorizeAsync(principal, Permissions.User.Delete);
            if(!isAdmin.Succeeded)
            {
                if(user.Id == loggedInUser.Id)
                    return (true, null, user, true, true, false); // ALLOW: We're editing ourself.

                // If we're not an admin, and if we're not managing ourself, check if we can at least read users.
                var canReadUsers = await this._auth.AuthorizeAsync(principal, Permissions.User.Read);
                if(canReadUsers.Succeeded)
                    return (true, null, user, false, canDeleteUsers.Succeeded, false);

                return (false, "You do not have permission to edit other users.", user, false, false, false);
            }

            return (true, null, user, false, true, true); // ALLOW: We're an admin.
        }
    }
}