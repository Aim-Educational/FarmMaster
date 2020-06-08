using FarmMasterTests.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FarmMasterTests.Integration
{
    public class AuthTests : IntegrationTest
    {
        static readonly IEnumerable<string> ANONYMOUS_WHITELIST = new List<string>(){ 
            // Format = Controller:Action:GET/POST
            "Account:Module:ConfirmEmail:GET",
            "Account:Module:ResendEmail:GET",
            "Account:Module:ResendEmail:POST",
            "Account:Module:Login:GET",
            "Account:Module:Login:POST",
            "Account:Module:Logout:GET",
            "Account:Module:Register:GET",
            "Account:Module:Register:POST",
            "Account:Module:ExternalLogin:GET",
            "Account:Module:HandleExternalLogin:GET",
            "Account:Module:FinaliseExternalLogin:GET",
            "Account:Module:FinaliseExternalLogin:POST",
            "Admin:Module:ManageUser:GET",  // Performs auth after being called
            "Admin:Module:DeleteUser:GET",  // ^^
            "Admin:Module:ManageUser:POST", // ^^
            "AzureAD:Account:SignIn:GET",
            "AzureAD:Account:SignOut:GET",
            "AzureAD:::GET", // Don't ask, cus I don't know.
            "Identity:::GET"
        };

        public AuthTests(ITestOutputHelper helper) : base(helper)
        {

        }

        /// <summary>
        /// Checks that all routes either: require authorisation, or are whitelisted to not require authorisation.
        /// 
        /// Purpose: Ensuring we don't accidentally leave any holes for unauthorised users to sneak through.
        /// </summary>
        [Fact()]
        public void CheckRoutesHaveAuthorised()
        {
            var actions = base.Host.Services.GetRequiredService<IActionDescriptorCollectionProvider>();

            var routes        = actions.ActionDescriptors.Items;
            var routesChecked = new List<string>();
            var routesInvalid = new List<string>();
            foreach (var route in routes)
            {
                // Note: For some reason, every route has [AllowAnonymous] attached to it (usually overriden by [Authorise] though)
                //       so keep that in mind.

                var isPost       = route.EndpointMetadata.Any(m => m is HttpPostAttribute);
                var hasAuthorise = route.EndpointMetadata.Any(m => m is AuthorizeAttribute);
                var area         = route.RouteValues["area"];
                var controller   = route.RouteValues["controller"];
                var action       = route.RouteValues["action"];

                if (hasAuthorise)
                    continue;

                var stringToCheck = $"{area}:{controller}:{action}:{(isPost ? "POST" : "GET")}";

                if (!ANONYMOUS_WHITELIST.Contains(stringToCheck))
                    routesInvalid.Add(stringToCheck);
                else
                    routesChecked.Add(stringToCheck);
            }

            if (routesInvalid.Any())
            {
                throw new XunitException(
                    $"The following routes don't require authorization, yet are not whitelisted: \n[\n" +
                    $"{routesInvalid.Aggregate((a, b) => $"    {a}\n    {b}")}\n" +
                    $"]"
                );
            }

            // Make sure we don't have any orphaned routes in the whitelist
            foreach (var whitelist in ANONYMOUS_WHITELIST)
                Assert.Contains(whitelist, routesChecked);
        }

        /// <summary>
        /// Checks that a logged out user is redirected to the login page if they fail an auth check.
        /// 
        /// Purpose: Just a general sanity check.
        /// </summary>
        [Fact()]
        public async Task CheckLoggedOutRedirect()
        {
            await base.Client.GetEnsureStatusAsync(
                "/Admin/Users", 
                HttpStatusCode.Redirect,
                new Regex(@"/Account/Login")
            );
        }

        /// <summary>
        /// Checks that an logged in, but unauthoirsed user is redirected to the access denied page if they fail an auth check.
        /// 
        /// Purpose: Just a general sanity check.
        /// </summary>
        [Fact()]
        public async Task CheckUnauthorisedRedirect()
        {
            await base.Client.SignupAsync("Andy", "Smells123");
            await base.Client.GetEnsureStatusAsync(
                "/Breed", 
                HttpStatusCode.Redirect,
                new Regex(@"/Account/AccessDenied")
            );
        }

        /// <summary>
        /// Checks that we can login.
        /// 
        /// Purpose: Basic foundational test required for any other tests that require authentication.
        /// </summary>
        [Fact()]
        public async Task CanLogin()
        {
            await base.Client.LoginAsync();
            await base.Client.GetEnsureStatusAsync("/Admin/Users", HttpStatusCode.OK);
        }

        /// <summary>
        /// Checks that we can register accounts.
        /// 
        /// Purpose: Basic foundational test requires for any other tests that require non-admin-level authorization.
        /// </summary>
        [Fact()]
        public async Task CanRegister()
        {
            await base.Client.SignupAsync("Andy", "Smells123");
            await base.Client.GetEnsureStatusAsync("/Admin/ManageUser?userId=2", HttpStatusCode.OK); // We can access our profile no matter what.
        }
    }
}
