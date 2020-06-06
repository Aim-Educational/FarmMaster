using DataAccess;
using GraphQL;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataAccessGraphQL
{
    // God imagine if stuff was simple.
    /**
     * Basically:
     * 
     *  - This is a service that the RootBase populates every request.
     *  
     *  - This allows graph types and the like to easily access the user, so they can perform auth checks.
     * */
    public class GraphQLUserContextAccessor
    {
        public DataAccessUserContext Context { get; set; }
    }

    // This is just so I can access the ApplicationUser easily
    public class DataAccessUserContext
    {
        public ApplicationUser UserIdentity { get; private set; }
        public ClaimsPrincipal UserPrincipal { get; private set; }
        public IAuthorizationService Auth { get; set; }

        public DataAccessUserContext(ApplicationUser UserIdentity, ClaimsPrincipal UserPrincipal, IAuthorizationService Auth)
        {
            this.UserIdentity = UserIdentity;
            this.UserPrincipal = UserPrincipal;
            this.Auth = Auth;
        }

        public async Task EnforceHasPolicyAsync(string perm)
        {
            var result = await this.Auth.AuthorizeAsync(this.UserPrincipal, perm);
            if (!result.Succeeded)
                throw new ExecutionError($"Missing permission {perm}");
        }
    }
}
