using AccountModule.Models;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FarmMasterTests.Common
{
    public partial class FarmClient
    {
        public async Task LoginAsync(string username = IdentityContext.DEFAULT_USERNAME, string password = IdentityContext.DEFAULT_PASSWORD)
        {
            var loginModel = new AccountLoginViewModel
            {
                Username = username,
                Password = password,
                RememberMe = true
            };

            await this.PostEnsureStatusAsync(
                "/Account/Login",
                new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { nameof(AccountLoginViewModel.Username),   loginModel.Username },
                    { nameof(AccountLoginViewModel.Password),   loginModel.Password },
                    { nameof(AccountLoginViewModel.RememberMe), loginModel.RememberMe ? "true" : "false" }
                }),
                HttpStatusCode.Redirect
            );
        }

        public async Task SignupAsync(string username, string password)
        {
            var signupModel = new AccountRegisterViewModel
            {
                Username = username,
                Password = password,
                Email = "no@example.com",
                ConfirmPassword = password
            };

            await this.PostEnsureStatusAsync(
                "/Account/Register",
                new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { nameof(AccountRegisterViewModel.Username),        signupModel.Username },
                    { nameof(AccountRegisterViewModel.Password),        signupModel.Password },
                    { nameof(AccountRegisterViewModel.Email),           signupModel.Email },
                    { nameof(AccountRegisterViewModel.ConfirmPassword), signupModel.ConfirmPassword }
                }),
                HttpStatusCode.Redirect
            );

            // Forcefully confirm email.
            var userManager = this._server.Services.GetRequiredService<UserManager<ApplicationUser>>();
            var user        = await userManager.FindByNameAsync(username);
            var token       = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var result      = await userManager.ConfirmEmailAsync(user, token);

            Assert.True(result.Succeeded);

            // Log in
            await this.LoginAsync(username, password);
        }
    }
}
