﻿using DataAccess;
using DataAccess.Constants;
using DataAccessLogic;
using System.Linq;

namespace DataAccessGraphQL.RootResolvers
{
    public class SpeciesRootResolver : CrudRootResolver<Species>
    {
        // So we don't constantly create the object.
        static readonly CrudRootResolverConfig CONFIG_INSTANCE = new CrudRootResolverConfig()
        {
            ReadPolicy = Permissions.Species.Read
        };
        protected override CrudRootResolverConfig Config => CONFIG_INSTANCE;

        public SpeciesRootResolver(ISpeciesManager species) : base(species)
        {
        }

        protected override IQueryable<Species> OrderPageQuery(IQueryable<Species> query, string order)
        {
            if (order == "id")
                query = query.OrderBy(s => s.SpeciesId);

            query.AddTableOrderBy(order, "name", s => s.Name);

            return query;
        }
    }
}
