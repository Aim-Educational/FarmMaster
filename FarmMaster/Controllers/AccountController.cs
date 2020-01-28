using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Filters;
using FarmMaster.Misc;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FarmMaster.Controllers
{
    public class AccountController : Controller
    {
        readonly IServiceUserManager _users;
        readonly IServiceContactManager _contacts;
        readonly IServiceGdpr _gdpr;
        readonly IServiceSmtpClient _smtp;
        readonly IOptions<IServiceSmtpDomainConfig> _domains;

        public AccountController(
            IServiceUserManager users, 
            IServiceContactManager userData, 
            IServiceGdpr gdpr,
            IServiceSmtpClient smtp,
            IOptions<IServiceSmtpDomainConfig> domains
        )
        {
            this._users = users;
            this._contacts = userData;
            this._gdpr = gdpr;
            this._smtp = smtp;
            this._domains = domains;
        }

        #region Profile GET
        [FarmAuthorise]
        public IActionResult Profile()
        {
            return View();
        }

        [FarmAuthorise]
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

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ResetPassword(string token)
        {
            return View(new AccountResetPasswordViewModel
            {
                Token = token
            });
        }
        #endregion

        #region Signup/in/out POST
        [HttpPost]
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

        [HttpPost]
        public IActionResult ForgotPassword(AccountForgotPasswordViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.ParseInvalidModelState(ModelState);
                return View(model);
            }

            var user = this._users.Query()
                                  .Include(u => u.Contact)
                                   .ThenInclude(c => c.EmailAddresses)
                                  .Include(u => u.Contact)
                                   .ThenInclude(c => c.Tokens)
                                  .Include(u => u.UserLoginInfo)
                                  .Where(u => u.UserLoginInfo.Username == model.Username)
                                  .FirstOrDefault();
            if(user == null)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateErrorQueryString($"No user called '{model.Username}' was found"));
                return View(model);
            }

            if(!user.Contact.EmailAddresses.Any(e => e.Address == model.Email))
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateErrorQueryString($"Email address '{model.Email}' does not belong to user '{model.Username}'"));
                return View(model);
            }

            var token = this._contacts.GenerateToken(
                user.Contact,
                ContactToken.Type.ResetPassword,
                DateTimeOffset.UtcNow + TimeSpan.FromHours(2),
                IsUnique.Yes
            );

            this._smtp.SendToWithTemplateAsync(
                new[] { model.Email },
                FarmConstants.EmailTemplateNames.ResetPasswordRequest,
                "Request to reset your password",
                ( 
                    callback: this._domains.Value.ResetPassword + token.Token,
                    username: user.UserLoginInfo.Username
                )
            ).Wait();

            return Redirect("/");
        }

        [HttpPost]
        public IActionResult ResetPassword(AccountResetPasswordViewModel model)
        {
            if(model.Password != model.PasswordConfirm)
                ModelState.AddModelError(nameof(model.PasswordConfirm), "The passwords don't match.");

            if (!ModelState.IsValid)
            {
                model.ParseInvalidModelState(ModelState);
                return View(model);
            }

            var contact = this._contacts.GetContactFromTokenString(model.Token);
            if(contact == null)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateErrorQueryString("Invalid or Expired token, please request a new password reset."));
                return View(model);
            }

            var user = this._users.Query()
                                  .Include(u => u.UserLoginInfo)
                                  .FirstOrDefault(u => u.ContactId == contact.ContactId);
            if(user == null)
            {
                model.Password = "";
                model.PasswordConfirm = "";
                throw new InvalidOperationException("Internal Error");
            }

            this._contacts.ExpireTokenByTokenString(contact, model.Token);
            this._users.ChangePassword(user, model.Password);
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

        public IActionResult AnonymiseRequest([FromQuery] string token)
        {
            var contact = this._contacts.GetContactFromTokenString(token);
            if(contact == null)
                return Redirect("/");

            this._contacts.ExpireTokenByTokenString(contact, token);
            
            if(contact.ContactType != Contact.Type.User)
                this._gdpr.AnonymiseContact(contact);
            else
            {
                var user = this._users.QueryAllIncluded()
                                      .First(u => u.ContactId == contact.ContactId);
                this._gdpr.AnonymiseUser(user);
            }

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
