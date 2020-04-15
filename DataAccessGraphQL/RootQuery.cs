using DataAccess;
using DataAccess.Constants;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessGraphQL
{
    public class DataAccessRootQuery : RootBase
    {
        readonly RootResolver<DataAccessUserContext> _userResolver;
        readonly RootResolver<Contact>               _contactResolver;
        readonly RootResolver<Species>               _speciesResolver;
        readonly RootResolver<Breed>                 _breedResolver;

        public DataAccessRootQuery(
            // Required for RootBase
            IHttpContextAccessor         context, 
            UserManager<ApplicationUser> users,
            GraphQLUserContextAccessor   accessor,
            FarmMasterContext            fmDb,
            IdentityContext              idDb,
            IAuthorizationService        auth,

            // Custom
            RootResolver<DataAccessUserContext> userResolver,
            RootResolver<Contact>               contactResolver,
            RootResolver<Species>               speciesResolver,
            RootResolver<Breed>                 breedResolver
        ) : base(context, users, accessor, fmDb, idDb, auth)
        {
            this._userResolver    = userResolver;
            this._contactResolver = contactResolver;
            this._speciesResolver = speciesResolver;
            this._breedResolver   = breedResolver;

            this.AddUserQuery();
            this.AddPermissionsQuery();
            this.AddContactQuery();
            this.AddSpeciesQuery();
            this.AddBreedQuery();
        }

        private void DefineSingleAndConnection<TGraphType, TSourceType>(string name, RootResolver<TSourceType> resolver)
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

        private void AddUserQuery()
        {
            this.DefineSingleAndConnection<UserGraphType, DataAccessUserContext>("user", this._userResolver);
        }

        private void AddPermissionsQuery()
        {
            Field<ListGraphType<StringGraphType>>(
                "permissions",
                "All permissions accepted by GraphQL.",
                resolve: ctx => Permissions.AllPermissions
            );
        }

        private void AddContactQuery()
        {
            this.DefineSingleAndConnection<ContactGraphType, Contact>("contact", this._contactResolver);
        }

        private void AddSpeciesQuery()
        {
            this.DefineSingleAndConnection<SpeciesGraphType, Species>("species", this._speciesResolver);
        }

        private void AddBreedQuery()
        {
            this.DefineSingleAndConnection<BreedGraphType, Breed>("breed", this._breedResolver);
        }
    }
}
