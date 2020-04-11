using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FarmMasterTests.Integration
{
    public class AuthTests : IntegrationTest
    {
        static readonly IEnumerable<string> ANONYMOUS_WHITELIST = new List<string>(){ 
            // Format = Controller:Action:GET/POST
            "Account:ConfirmEmail:GET",
            "Account:ResendEmail:GET",
            "Account:ResendEmail:POST",
            "Account:Login:GET",
            "Account:Login:POST",
            "Account:Logout:GET",
            "Account:Register:GET",
            "Account:Register:POST",
            "Account:ExternalLogin:GET",
            "Account:HandleExternalLogin:GET",
            "Account:FinaliseExternalLogin:GET",
            "Account:FinaliseExternalLogin:POST"
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
            foreach(var route in routes)
            {
                if(route.AttributeRouteInfo != null) // This skips all of the premade Identity pages.
                    continue;

                // Note: For some reason, every route has [AllowAnonymous] attached to it (usually overriden by [Authorise] though)
                //       so keep that in mind.

                var isPost       = route.EndpointMetadata.Any(m => m is HttpPostAttribute);
                var hasAuthorise = route.EndpointMetadata.Any(m => m is AuthorizeAttribute);
                var controller   = route.RouteValues["controller"];
                var action       = route.RouteValues["action"];

                if(hasAuthorise)
                    continue;

                var stringToCheck = $"{controller}:{action}:{(isPost ? "POST" : "GET")}";
                Assert.Contains(stringToCheck, ANONYMOUS_WHITELIST);

                routesChecked.Add(stringToCheck);
            }

            // Make sure we don't have any orphaned routes in the whitelist
            foreach(var whitelist in ANONYMOUS_WHITELIST)
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
            var response = await base.Client.GetEnsureStatusAsync("/Admin/Users", HttpStatusCode.Redirect);
            Assert.Contains("Account/Login", response.Headers.Location.ToString());
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
    }
}
