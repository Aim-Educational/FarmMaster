﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public AccountController(IServiceUserManager users)
        {
            this._users = users;
        }

        public IActionResult Login([FromQuery] bool? verifyEmail)
        {
            return View(new AccountLoginViewModel{ VerifyEmail = verifyEmail.GetValueOrDefault(false) });
        }

        public IActionResult Signup()
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
                return View(model);

            this._users.CreateUser(model.LoginInfo.Username,                model.LoginInfo.Password,
                                   model.Contact.FirstName,                 model.Contact.MiddleNames,
                                   model.Contact.LastName,                  model.Contact.Email,
                                   model.ConsentInfo.TermsOfServiceConsent, model.ConsentInfo.PrivacyPolicyConsent);
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