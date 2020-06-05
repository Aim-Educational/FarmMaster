using DataAccess;
using DataAccess.Constants;
using DataAccessGraphQL.Api;
using DataAccessGraphQL.Constants;
using DataAccessGraphQL.GraphTypes;
using DataAccessGraphQL.RootResolvers;
using DataAccessGraphQL.Util;
using GraphQL;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessGraphQL
{
    public class RootGraphQLQuery : RootBase
    {
        public RootGraphQLQuery(
            // Required for RootBase
            IHttpContextAccessor                context, 
            UserManager<ApplicationUser>        users,
            GraphQLUserContextAccessor          accessor,
            FarmMasterContext                   fmDb,
            IdentityContext                     idDb,
            IAuthorizationService               auth,

            // Extern providers
            IEnumerable<IGraphQLQueryProvider>  providers
        ) : base(context, users, accessor, fmDb, idDb, auth)
        {
            foreach(var provider in providers)
                provider.AddQueries(this);
            //this._userResolver    = userResolver;
            //this._contactResolver = contactResolver;
            //this._speciesResolver = speciesResolver;
            //this._breedResolver   = breedResolver;

            //this.AddPermissionsQuery();
            
            //this.DefineSingleAndConnection<UserGraphType,    DataAccessUserContext>("user",    this._userResolver);
            //this.DefineSingleAndConnection<ContactGraphType, Contact>              ("contact", this._contactResolver);
            //this.DefineSingleAndConnection<BreedGraphType,   Breed>                ("breed",   this._breedResolver);
            //this.DefineSingleAndConnection<SpeciesGraphType, Species>              ("species", this._speciesResolver);
        }

        public void DefineSingleAndConnection<TGraphType, TSourceType>(string name, RootResolver<TSourceType> resolver)
        where TGraphType : GraphType
        where TSourceType : class
        {
            var namePlural = (name.EndsWith("s"))
                             ? name + "es" // species -> specieses. Since we *have* to differentiate, even if not gramatically correct.
                             : name + "s"; // contact -> contacts.

            FieldAsync<TGraphType>(
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

        private void AddPermissionsQuery()
        {
            Field<ListGraphType<StringGraphType>>(
                "permissions",
                "All permissions accepted by GraphQL.",
                resolve: ctx => Permissions.AllPermissions
            );
        }
    }
}
