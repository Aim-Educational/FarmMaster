using DataAccess;
using DataAccessGraphQL.GraphTypes;
using DataAccessGraphQL.Mutations;
using DataAccessGraphQL.RootResolvers;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessGraphQL
{
    // Meh, code duplication between query and mutation.
    public class DataAccessRootMutation : RootBase
    {
        readonly RootResolver<DataAccessUserContext> _userResolver;
        readonly RootResolver<Contact>               _contactResolver;
        readonly RootResolver<Species>               _speciesResolver;
        readonly RootResolver<Breed>                 _breedResolver;

        public DataAccessRootMutation(
            IHttpContextAccessor         context, 
            UserManager<ApplicationUser> users,
            GraphQLUserContextAccessor   accessor,
            FarmMasterContext            fmDb,
            IdentityContext              idDb,
            IAuthorizationService        auth,
            
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

            this.AddGenericMutationAsync<UserRootMutation,    DataAccessUserContext>("user",    this._userResolver);
            this.AddGenericMutationAsync<ContactRootMutation, Contact>              ("contact", this._contactResolver);
            this.AddGenericMutationAsync<BreedRootMutation,   Breed>                ("breed",   this._breedResolver);
            this.AddGenericMutationAsync<SpeciesRootMutation, Species>              ("species", this._speciesResolver);
        }

        private void AddGenericMutationAsync<TMutation, TResolverSource>(
            string name,
            RootResolver<TResolverSource> resolver
        )
        where TMutation : GraphType
        where TResolverSource : class
        {
            FieldAsync<TMutation>(
                name,
                arguments: resolver,
                resolve: ctx => resolver.ResolveAsync(ctx, base.DataContext)
            );
        }
    }
}
