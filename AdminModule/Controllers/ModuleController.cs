using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccessLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DataAccess.Constants;
using EmailSender;
using Microsoft.Extensions.DependencyInjection;
using AccountModule.Constants;
using AdminModule.Models;

namespace AdminModule.Controllers
{
    [Area("Admin")]
    [Route("/Admin/{action}")]
    [Authorize(Policy = Policies.SeeAdminPanel)]
    public class ModuleController : Controller
    {
        const string VIEW_CONTROL_TEST = "~/Views/AdminModule/ControlTest.cshtml";
        const string VIEW_SETTINGS     = "~/Views/AdminModule/Settings.cshtml";

        [Authorize(Policy = Permissions.Other.DebugUI)]
        public IActionResult ControlTest()
        {
            return View(VIEW_CONTROL_TEST);
        }

        [Authorize(Policy = Permissions.Other.Settings)]
        public IActionResult Settings([FromServices] IOptionsSnapshot<EmailSenderConfig> emailConf, [FromQuery] string emailTestError)
        {
            return View(VIEW_SETTINGS, new AdminSettingsViewModel
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
                return View(VIEW_SETTINGS, settings);
            }

            return RedirectToAction("Settings");
        }
    }
}