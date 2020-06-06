using DataAccess;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace DataAccessGraphQL
{
    // Trust me, I hate this almost as much as you do.
    public abstract class RootBase : ObjectGraphType<object>
    {
        public DataAccessUserContext DataContext { private set; get; }
        public FarmMasterContext FarmMasterDb { private set; get; }
        public IdentityContext IdentityDb { private set; get; }
        public UserManager<ApplicationUser> UserManager { private set; get; }

        public RootBase(
            IHttpContextAccessor context,
            UserManager<ApplicationUser> users,
            GraphQLUserContextAccessor accessor,
            FarmMasterContext fmDb,
            IdentityContext idDb,
            IAuthorizationService auth
        )
        {
            var user = users.GetUserAsync(context.HttpContext.User).Result;
            accessor.Context = new DataAccessUserContext(user, context.HttpContext.User, auth);
            if (user == null)
                throw new ExecutionError("You are not logged in");

            this.DataContext = accessor.Context;
            this.FarmMasterDb = fmDb;
            this.IdentityDb = idDb;
            this.UserManager = users;
        }
    }
}
