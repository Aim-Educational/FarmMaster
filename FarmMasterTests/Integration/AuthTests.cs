using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

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
    }
}
