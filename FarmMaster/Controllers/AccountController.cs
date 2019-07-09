using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FarmMaster.Misc;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmMaster.Controllers
{
    public class AccountController : Controller
    {
        readonly IServiceUserManager _users;
        readonly IServiceContactData    _userData;

        public AccountController(IServiceUserManager users, IServiceContactData userData)
        {
            this._users = users;
            this._userData = userData;
        }

        public IActionResult Login([FromQuery] bool? verifyEmail)
        {
            return View(new AccountLoginViewModel{ VerifyEmail = verifyEmail.GetValueOrDefault(false) });
        }

        public IActionResult Signup()
        {
            return View();
        }

        public IActionResult Logout()
        {
            var user = this._users.UserFromCookieSession(HttpContext);
            if(user == null)
                return RedirectToAction(nameof(Login));

            this._users.EndSession(user, HttpContext);
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult ResendEmailVerifyEmail()
        {
            var user = this._users.UserFromCookieSession(this.HttpContext);
            if(user != null)
                this._users.SendEmailVerifyEmail(user);

            return RedirectToAction(nameof(Login), new { verifyEmail = true });
        }

        public IActionResult VerifyEmail([FromQuery] string token)
        {
            this._users.FinishEmailVerify(token);
            return Redirect("/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(AccountSignupViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.MessageFormat = ViewModelWithMessage.Format.UnorderedList;
                model.MessageType = ViewModelWithMessage.Type.Error;
                model.Message = ViewModelWithMessage.CreateMessageQueryString(ModelState).Substring(1); // The substring is to cut off the error type mark.
                return View(model);
            }

            try
            {
                if(!model.ConsentInfo.AgeConsent)
                    throw new Exception("You must state that you're 13 or over before you can sign up.");

                var user = this._users.CreateUser(model.LoginInfo.Username, 
                                                  model.LoginInfo.Password,
                                                  model.NameInfo.FirstName + model.NameInfo.MiddleNames + model.NameInfo.LastName,
                                                  model.Email,
                                                  model.ConsentInfo.TermsOfServiceConsent, 
                                                  model.ConsentInfo.PrivacyPolicyConsent);

                // TODO: Support multiple number entries.
                this._userData.AddTelephoneNumber(user.Contact, GlobalConstants.DefaultNumberName, model.TelephoneNumbers[0]);
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
    }
}
