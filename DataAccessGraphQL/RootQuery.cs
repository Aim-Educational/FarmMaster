using DataAccess;
using DataAccess.Constants;
using DataAccessGraphQL.Api;
using DataAccessGraphQL.RootResolvers;
using DataAccessGraphQL.Util;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DataAccessGraphQL
{
    public class RootGraphQLQuery : RootBase
    {
        public RootGraphQLQuery(
            // Required for RootBase
            IHttpContextAccessor context,
            UserManager<ApplicationUser> users,
            GraphQLUserContextAccessor accessor,
            FarmMasterContext fmDb,
            IdentityContext idDb,
            IAuthorizationService auth,

            // Extern providers
            IEnumerable<IGraphQLQueryProvider> providers
        ) : base(context, users, accessor, fmDb, idDb, auth)
        {
            foreach (var provider in providers)
                provider.AddQueries(this);
        }

        public void DefineSingleAndConnection<TGraphType, TSourceType>(string name, RootResolver<TSourceType> resolver)
        where TGraphType : GraphType
        where TSourceType : class
        {
            var namePlural = (name.EndsWith("s"))
                             ? name + "es" // species -> specieses. Since we *have* to differentiate, even if not gramatically correct.
                             : name + "s"; // contact -> contacts.

            this.FieldAsync<TGraphType>(
                name,
                arguments: resolver,
                resolve: ctx => resolver.ResolveAsync(ctx, base.DataContext)
            );

            this.DefineConnectionAsync<object, TGraphType, TSourceType>(
                namePlural,
                base.DataContext,
                (ctx, first, after, order) => resolver.ResolvePageAsync(ctx, first, after, order)
            );
        }
    }
}
