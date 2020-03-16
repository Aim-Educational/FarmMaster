using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        public IActionResult Login([FromQuery] bool confirmEmail, [FromQuery] string error)
        {
            return View(new AccountLoginViewModel
            {
                ConfirmEmail = confirmEmail,
                Error = error
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

        /**
         * The actual flow of external logins seems *very, very* poorly documented, so here it is:
         * 
         * - User clicks "Sign in with Azure", which calls this action where `provider` = "AzureAD" (matching the auth scheme for Azure).
         * 
         * - This action creates a challenge for the AzureAD scheme, providing it a redirect url (GET) for `HandleExternalLogin`.
         * 
         * - Before HandleExternalLogin is called, I think the authentication middleware gives the AzureAD scheme a chance
         *   to process some info (literally 0 documentation on this that I can find).
         *   
         * - HandleExternalLogin will then check with the signInManager (who somehow has this info?) whether the user was logged in
         *   or not.
         *   
         *   - HandleExternalLogin will then attempt to login the user, redirecting them to FinaliseExternalLogin if they don't
         *     have an account on our end yet.
         *     
         * - FinaliseExternalLogin will then, somehow with access still, create a link between the user account on our end
         *   and the user account on azure's end.
         * **/
        public IActionResult ExternalLogin([FromQuery] string provider, [FromQuery] string returnUrl)
        {
            var callbackUrl = Url.Action("HandleExternalLogin", "Account", new { returnUrl });
            var properties  = this._signInManager.ConfigureExternalAuthenticationProperties(provider, callbackUrl);

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> HandleExternalLogin([FromQuery] string returnUrl, [FromQuery] string remoteError)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
                return RedirectToAction("Login", new { ReturnUrl = returnUrl, error = $"Error from external provider: {remoteError}" });

            // It's kind of hard to follow (and very poorly documented), but I think the authentication middleware 
            // handles part of this request prior to this action being called?
            var info = await this._signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Login", new { ReturnUrl = returnUrl, error = "Error loading external login information." });

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
                return LocalRedirect("/Identity/Account/Lockout");
            else
            {
                // If the user needs to create an account, send them to FinaliseExternalLogin.
                var email    = info.Principal.FindFirst(ClaimTypes.Email)?.Value;

                var username = info.Principal.FindFirst("Name")?.Value
                            ?? info.Principal.FindFirst(ClaimTypes.Name)?.Value;

                return RedirectToAction("FinaliseExternalLogin", new { email, username });
            }
        }

        public IActionResult FinaliseExternalLogin([FromQuery] string username, [FromQuery] string email)
        {
            return View(new AccountFinaliseExternalLoginViewModel
            {
                Username = username,
                Email = email
            });
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
                var user   = new ApplicationUser { UserName = model.Username, Email = model.Email };
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

        [HttpPost]
        public async Task<IActionResult> FinaliseExternalLogin(AccountFinaliseExternalLoginViewModel model, [FromQuery] string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            // Get the information about the user from the external login provider
            var info = await this._signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl, error = "Error loading external login information during confirmation." });

            if (ModelState.IsValid)
            {
                var user   = new ApplicationUser { UserName = model.Username, Email = model.Email };
                var result = await this._userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await this._userManager.AddLoginAsync(user, info);

                    if (result.Succeeded)
                    {
                        await this._signInManager.SignInAsync(user, isPersistent: false);
                        this._logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await this._userManager.GetUserIdAsync(user);
                        var token  = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                            token  = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                        var callbackUrl = Url.Action(
                            "ConfirmEmail",
                            "Account",
                            protocol: Request.Scheme,
                            values: new { userId, token }
                        );

                        await this._emailSender.SendEmailAsync(
                            model.Email,
                            "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
                        );

                        return RedirectToAction("Login", new { confirmEmail = true });
                    }
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}