using DataAccess;
using DataAccess.Constants;
using DataAccessLogic;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessGraphQL.RootResolvers
{
    public class BreedRootResolver : RootResolver<Breed>
    {
        readonly IBreedManager _breeds;

        public BreedRootResolver(IBreedManager breeds)
        {
            this._breeds = breeds;

            base.Add(new QueryArgument<NonNullGraphType<IdGraphType>>
            {
                Name = "id",
                Description = "Get a breed by their ID"
            });
        }

        public override Task<object> ResolveAsync(
            IResolveFieldContext<object> context,
            DataAccessUserContext userContext
        )
        {
            return base.ResolveCrudAsync(Permissions.Breed.Read, this._breeds, context, userContext);
        }

        public override Task<IEnumerable<Breed>> ResolvePageAsync(
            DataAccessUserContext userContext, 
            int first, 
            int after, 
            string order
        )
        {
            // The dynamic ordering is a bit too annoying to express with Managers, so we're accessing .Query
            var query = this._breeds.IncludeAll(this._breeds.Query());
            if(order == "id")
                query = query.OrderBy(c => c.BreedId);

            return Task.FromResult(
                        query.Skip(after)
                             .Take(first)
                             .AsEnumerable()
            );
        }
    }
}
