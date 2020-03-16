using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DataAccess;
using FarmMaster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace FarmMaster.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        readonly SignInManager<ApplicationUser> _signInManager;
        readonly UserManager<ApplicationUser> _userManager;
        readonly IEmailSender _emailSender;
        readonly ILogger _logger;

        public AccountController(
            SignInManager<ApplicationUser> signIn,
            UserManager<ApplicationUser> users,
            IEmailSender email,
            ILogger<AccountController> logger
        )
        {
            this._signInManager = signIn;
            this._userManager = users;
            this._logger = logger;
            this._emailSender = email;
        }

        public IActionResult Login([FromQuery] bool confirmEmail)
        {
            return View(new AccountLoginViewModel
            {
                ConfirmEmail = confirmEmail
            });
        }

        public async Task<IActionResult> Logout()
        {
            await this._signInManager.SignOutAsync();
            this._logger.LogInformation("User logged out.");
            
            return LocalRedirect("/");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountLoginViewModel model, [FromQuery] string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await this._signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    this._logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                else if (result.IsLockedOut)
                {
                    this._logger.LogWarning("User account locked out.");
                    return LocalRedirect("/Identity/Account/Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Have you confirmed your email?");
                    return View();
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountRegisterViewModel model, [FromQuery] string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if(model.Password != model.ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match");

            if (ModelState.IsValid)
            {
                var user   = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await this._userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    var callbackUrl = Url.Action(
                        "ConfirmEmail", 
                        "Account", 
                        protocol: Request.Scheme,
                        values: new { userId = user.Id, token }
                    );

                    await this._emailSender.SendEmailAsync(
                        model.Email, 
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
                    );

                    return RedirectToAction("Login", new { confirmEmail = true, returnUrl });
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }
    }
}