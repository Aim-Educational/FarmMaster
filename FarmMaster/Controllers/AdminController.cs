using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLogic;
using FarmMaster.Areas.Identity.Services;
using FarmMaster.Constants;
using FarmMaster.Models;
using Microsoft.AspNetCore.Authorization;
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