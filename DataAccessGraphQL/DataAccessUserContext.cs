using DataAccess;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DataAccessGraphQL
{
    // God imagine if stuff was simple.
    /**
     * Basically:
     * 
     *  - This is a service that the RootQuery populates every request.
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
        public ApplicationUser UserIdentity  { get; private set; }
        public ClaimsPrincipal UserPrincipal { get; private set; }

        public DataAccessUserContext(ApplicationUser UserIdentity, ClaimsPrincipal UserPrincipal)
        {
            this.UserIdentity = UserIdentity;
            this.UserPrincipal = UserPrincipal;
        }
    }
}
