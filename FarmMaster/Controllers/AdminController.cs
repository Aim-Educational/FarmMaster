using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccessLogic;
using FarmMaster.Areas.Identity.Services;
using FarmMaster.Constants;
using FarmMaster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FarmMaster.Controllers
{
    [Authorize(Policy = PolicyNames.IS_ADMIN)]
    public class AdminController : Controller
    {
        public IActionResult ControlTest()
        {
            return View();
        }

        public IActionResult Settings([FromServices] IOptionsSnapshot<EmailSenderConfig> emailConf)
        {
            return View(new AdminSettingsViewModel
            { 
                Email = emailConf.Value
            });
        }

        public async Task<IActionResult> TestEmail([FromServices] IEmailSender email, [FromServices] UserManager<ApplicationUser> users)
        {
            var user    = await users.GetUserAsync(User);
            var address = await users.GetEmailAsync(user);

            await email.SendEmailAsync(address, "This is a test", "<h1>Lalafells are demons, change my mind</h1>");

            return RedirectToAction("Settings");
        }

        [HttpPost]
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
    }
}