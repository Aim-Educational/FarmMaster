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
    public class SpeciesRootResolver : RootResolver<Species>
    {
        readonly ISpeciesManager _species;

        public SpeciesRootResolver(ISpeciesManager species)
        {
            this._species = species;

            base.Add(new QueryArgument<NonNullGraphType<IdGraphType>>
            {
                Name = "id",
                Description = "Get a species by their ID"
            });
        }

        public override async Task<object> ResolveAsync(
            IResolveFieldContext<object> context,
            DataAccessUserContext userContext
        )
        {
            await userContext.EnforceHasPolicyAsync(Permissions.Species.Read);

            // ARGS
            var id = context.GetArgument<int>("id");

            // Find the species
            var species = await this._species.GetByIdAsync(id);
            if(!species.Succeeded)
                throw new ExecutionError(species.GatherErrorMessages().Aggregate((a, b) => $"{a}\n{b}"));

            return species.Value;
        }

        public override async Task<IEnumerable<Species>> ResolvePageAsync(
            DataAccessUserContext userContext, 
            int first, 
            int after, 
            string order
        )
        {
            // The dynamic ordering is a bit too annoying to express with Managers, so we're accessing .Query
            var query = this._species.IncludeAll(this._species.Query());
            if(order == "id")
                query = query.OrderBy(c => c.SpeciesId);

            return query.Include(c => c.NoteOwner)
                         .ThenInclude(o => o.NoteEntries)
                        .Skip(after)
                        .Take(first);
        }
    }
}
