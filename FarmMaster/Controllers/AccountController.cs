using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FarmMaster.Filters;
using FarmMaster.Misc;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FarmMaster.Controllers
{
    public class AccountController : Controller
    {
        readonly IServiceUserManager _users;
        readonly IServiceContactManager _contacts;
        readonly IServiceGdpr _gdpr;

        public AccountController(IServiceUserManager users, IServiceContactManager userData, IServiceGdpr gdpr)
        {
            this._users = users;
            this._contacts = userData;
            this._gdpr = gdpr;
        }

        #region Profile GET
        [FarmAuthorise()]
        public IActionResult Profile()
        {
            return View();
        }

        [FarmAuthorise()]
        public IActionResult DownloadMyData()
        {
            var user = this._users.UserFromCookieSession(HttpContext);
            if(user == null)
                return NotFound();

            var json = this._gdpr.GetAggregatedDataForUser(user);
            var jsonBytes = Encoding.UTF8.GetBytes(json.ToString(Formatting.Indented));
            return File(jsonBytes, "text/json");
        }
        #endregion

        #region Profile POST
        #endregion

        #region Signup/in/out GET
        public IActionResult Login([FromQuery] bool? verifyEmail)
        {
            return View(new AccountLoginViewModel { VerifyEmail = verifyEmail.GetValueOrDefault(false) });
        }

        public IActionResult Signup()
        {
            return View();
        }

        public IActionResult Logout()
        {
            var user = this._users.UserFromCookieSession(HttpContext);
            if (user == null)
                return RedirectToAction(nameof(Login));

            this._users.EndSession(user, HttpContext);
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction(nameof(Login));
        }
        #endregion

        #region Signup/in/out POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public IActionResult Signup(AccountSignupViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.ParseInvalidModelState(ModelState);
                return View(model);
            }

            try
            {
                if(!model.ConsentInfo.AgeConsent)
                    throw new Exception("You must state that you're 13 or over before you can sign up.");

                var user = this._users.Create(
                    model.LoginInfo.Username, 
                    model.LoginInfo.Password,
                    model.NameInfo.FirstName + " " + model.NameInfo.MiddleNames + " " + model.NameInfo.LastName,
                    model.Email,
                    model.ConsentInfo.TermsOfServiceConsent, 
                    model.ConsentInfo.PrivacyPolicyConsent
                );

                // TODO: Support multiple number entries.
                this._contacts.AddTelephoneNumber(
                    user.Contact,
                    user,
                    "User signup requires a default phone number.",
                    "Default",
                    model.TelephoneNumbers[0]
                );
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(null, ex.Message);
                return View(model);
            }
            
            return RedirectToAction(nameof(Login), new { verifyEmail = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(AccountLoginViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            var user = this._users.UserFromLoginInfo(model.Username, model.Password);
            this._users.RenewSession(user, this.HttpContext);
            
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserLoginInfo.Username));

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity)).Wait();
            return Redirect("/");
        }
        #endregion

        #region Email callbacks and GET
        public IActionResult ResendEmailVerifyEmail()
        {
            var user = this._users.UserFromCookieSession(this.HttpContext);
            if (user != null)
                this._users.SendEmailVerifyEmail(user);

            return RedirectToAction(nameof(Login), new { verifyEmail = true });
        }

        public IActionResult VerifyEmail([FromQuery] string token)
        {
            this._users.FinishEmailVerify(token);
            return Redirect("/");
        }
        #endregion

        #region Other
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion
    }
}
