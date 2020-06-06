using DataAccess;
using DataAccessGraphQL.Api;
using DataAccessGraphQL.RootResolvers;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DataAccessGraphQL
{
    // Meh, code duplication between query and mutation.
    public class RootGraphQLMutation : RootBase
    {
        public RootGraphQLMutation(
            IHttpContextAccessor context,
            UserManager<ApplicationUser> users,
            GraphQLUserContextAccessor accessor,
            FarmMasterContext fmDb,
            IdentityContext idDb,
            IAuthorizationService auth,

            IEnumerable<IGraphQLMutationProvider> providers
        ) : base(context, users, accessor, fmDb, idDb, auth)
        {
            foreach (var provider in providers)
                provider.AddMutations(this);
        }

        public void AddGenericMutationAsync<TMutation, TResolverSource>(
            string name,
            RootResolver<TResolverSource> resolver
        )
        where TMutation : GraphType
        where TResolverSource : class
        {
            this.FieldAsync<TMutation>(
                name,
                arguments: resolver,
                resolve: ctx => resolver.ResolveAsync(ctx, base.DataContext)
            );
        }
    }
}
