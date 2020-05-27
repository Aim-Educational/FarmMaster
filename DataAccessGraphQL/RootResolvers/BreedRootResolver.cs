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
    public class BreedRootResolver : CrudRootResolver<Breed>
    {
        static readonly CrudRootResolverConfig CONFIG_INSTANCE = new CrudRootResolverConfig 
        {
            ReadPolicy = Permissions.Breed.Read
        };
        protected override CrudRootResolverConfig Config => CONFIG_INSTANCE;

        public BreedRootResolver(IBreedManager breeds) : base(breeds)
        {
        }

        protected override IQueryable<Breed> OrderPageQuery(IQueryable<Breed> query, string order)
        {
            if(order == "id")
                query = query.OrderBy(b => b.BreedId);

            query = query.AddTableOrderBy(order, "name",         b => b.Name);
            query = query.AddTableOrderBy(order, "species.name", b => b.Species.Name);

            return query;
        }
    }
}
