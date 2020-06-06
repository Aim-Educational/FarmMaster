using AccountModule.Constants;
using AccountModule.Models;
using DataAccess;
using EmailSender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AccountModule.Controllers
{
    [AllowAnonymous]
    [Area("Account")]
    [Route("/Account/{action}")]
    public class ModuleController : Controller
    {
        readonly SignInManager<ApplicationUser> _signInManager;
        readonly UserManager<ApplicationUser> _userManager;
        readonly ITemplatedEmailSender _emailSender;
        readonly ILogger _logger;

        public ModuleController(
            SignInManager<ApplicationUser> signIn,
            UserManager<ApplicationUser> users,
            ITemplatedEmailSender email,
            ILogger<ModuleController> logger
        )
        {
            this._signInManager = signIn;
            this._userManager = users;
            this._logger = logger;
            this._emailSender = email;
        }

        #region Confirm email. Resend confirmation.
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return this.RedirectToAction("Login", new { error = "Invalid query parameters" });

            var user = await this._userManager.FindByIdAsync(userId);
            if (user == null)
                return this.RedirectToAction("Login", new { error = $"No user with ID {userId} could be found" });

            var result = await this._userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return this.RedirectToAction("Login", new { success = true });

            var errorMessage = "";
            foreach (var error in result.Errors)
                errorMessage += error.Description + "; ";

            return this.RedirectToAction("Login", new { error = errorMessage });
        }

        public IActionResult ResendEmail()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmail(AccountResendEmailViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this._userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    this._logger.LogInformation("Resending confirmation to User {Name}", user.UserName);

                    var token = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = this.Url.Action(
                        "ConfirmEmail",
                        "Account",
                        protocol: this.Request.Scheme,
                        values: new { userId = user.Id, token }
                    );

                    await this._emailSender.SendConfirmPasswordAsync(model.Email, callbackUrl);
                    return this.RedirectToAction("Login", new { confirmEmail = true });
                }
                else
                    this.ModelState.AddModelError(string.Empty, "No account with that email was found.");
            }

            return this.View(model);
        }
        #endregion

        #region Login, logout, and Register
        public IActionResult Login([FromQuery] bool confirmEmail, [FromQuery] string error, [FromQuery] bool success)
        {
            return this.View(new AccountLoginViewModel
            {
                ConfirmEmail = confirmEmail,
                Error = error,
                Success = success
            });
        }

        public async Task<IActionResult> Logout()
        {
            await this._signInManager.SignOutAsync();
            this._logger.LogInformation("User logged out.");

            return this.LocalRedirect("/");
        }

        public IActionResult Register()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountLoginViewModel model, [FromQuery] string returnUrl)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.ModelState.IsValid)
            {
                var result = await this._signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    this._logger.LogInformation("User logged in.");
                    return this.LocalRedirect(returnUrl);
                }
                else if (result.IsLockedOut)
                {
                    this._logger.LogWarning("User account locked out.");
                    return this.LocalRedirect("/Identity/Account/Lockout");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt. Have you confirmed your email?");
                    return this.View();
                }
            }

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AccountRegisterViewModel model, [FromQuery] string returnUrl)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (model.Password != model.ConfirmPassword)
                this.ModelState.AddModelError("ConfirmPassword", "Passwords do not match");

            if (this.ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
                var result = await this._userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    this._logger.LogInformation("User created a new account with password.");

                    var token = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = this.Url.Action(
                        "ConfirmEmail",
                        "Account",
                        protocol: this.Request.Scheme,
                        values: new { userId = user.Id, token }
                    );

                    await this._emailSender.SendConfirmPasswordAsync(model.Email, callbackUrl);
                    return this.RedirectToAction("Login", new { confirmEmail = true, returnUrl });
                }

                foreach (var error in result.Errors)
                    this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.View();
        }
        #endregion

        #region External Login
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
            var callbackUrl = this.Url.Action("HandleExternalLogin", "Account", new { returnUrl });
            var properties = this._signInManager.ConfigureExternalAuthenticationProperties(provider, callbackUrl);

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> HandleExternalLogin(
            [FromQuery] string returnUrl,
            [FromQuery] string remoteError,
            [FromServices] IdentityContext db
        )
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");
            if (remoteError != null)
                return this.RedirectToAction("Login", new { ReturnUrl = returnUrl, error = $"Error from external provider: {remoteError}" });

            // It's kind of hard to follow (and very poorly documented), but I think the authentication middleware 
            // handles part of this request prior to this action being called?
            var info = await this._signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return this.RedirectToAction("Login", new { ReturnUrl = returnUrl, error = "Error loading external login information." });

            // Sign in the user with this external login provider if the user already has a login.
            var result = await this._signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                this._logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return this.LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
                return this.LocalRedirect("/Identity/Account/Lockout");
            else
            {
                // If the user exists, send them back to the login page with "Please check your email" alert
                // P.S. There's literally no simple way of getting a user from a provider key via userManager
                var externLogin = db.UserLogins.FirstOrDefault(l => l.ProviderKey == info.ProviderKey && l.LoginProvider == info.LoginProvider);

                if (externLogin != null)
                {
                    // If the user needs to just confirm their email, then send them back to the login page.
                    var user = db.Users.First(u => u.Id == externLogin.UserId);

                    if (!user.EmailConfirmed)
                        return this.RedirectToAction("Login", new { confirmEmail = true });
                    else
                        return this.RedirectToAction("Login", new { error = "You seem to be in a limbo, I don't know why you can't login." });
                }
                else
                {
                    // If the user needs to create an account, send them to FinaliseExternalLogin.
                    var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;

                    var username = info.Principal.FindFirst("Name")?.Value
                                ?? info.Principal.FindFirst(ClaimTypes.Name)?.Value;

                    return this.RedirectToAction("FinaliseExternalLogin", new { email, username });
                }
            }
        }

        public IActionResult FinaliseExternalLogin([FromQuery] string username, [FromQuery] string email)
        {
            return this.View(new AccountFinaliseExternalLoginViewModel
            {
                Username = username,
                Email = email
            });
        }

        [HttpPost]
        public async Task<IActionResult> FinaliseExternalLogin(AccountFinaliseExternalLoginViewModel model, [FromQuery] string returnUrl)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            // Get the information about the user from the external login provider
            var info = await this._signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return this.RedirectToAction("Login", new { ReturnUrl = returnUrl, error = "Error loading external login information during confirmation." });

            if (this.ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
                var result = await this._userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await this._userManager.AddLoginAsync(user, info);
                    await this._userManager.AddClaimAsync(user, new Claim(ClaimTypes.AuthenticationMethod, info.LoginProvider));

                    if (result.Succeeded)
                    {
                        await this._signInManager.SignInAsync(user, isPersistent: false);
                        this._logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await this._userManager.GetUserIdAsync(user);
                        var token = await this._userManager.GenerateEmailConfirmationTokenAsync(user);

                        var callbackUrl = this.Url.Action(
                            "ConfirmEmail",
                            "Account",
                            protocol: this.Request.Scheme,
                            values: new { userId, token }
                        );

                        await this._emailSender.SendEmailAsync(
                            model.Email,
                            "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
                        );

                        return this.RedirectToAction("Login", new { confirmEmail = true });
                    }
                }

                foreach (var error in result.Errors)
                    this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.View(model);
        }
        #endregion
    }
}