using DataAccess;
using DataAccess.Constants;
using DataAccessGraphQL.RootResolvers;
using DataAccessLogic;
using System.Linq;

namespace CrudModule.GraphQL.Resolvers
{
    public class LocationRootResolver : CrudRootResolver<Location>
    {
        static readonly CrudRootResolverConfig CONFIG_INSTANCE = new CrudRootResolverConfig
        {
            ReadPolicy = Permissions.Location.Read
        };
        protected override CrudRootResolverConfig Config => CONFIG_INSTANCE;

        public LocationRootResolver(ILocationManager locations) : base(locations)
        {
        }

        protected override IQueryable<Location> OrderPageQuery(IQueryable<Location> query, string order)
        {
            if (order == "id")
                query = query.OrderBy(b => b.LocationId);

            query = query.TableOrderBy(order, "name", b => b.Name);
            query = query.TableOrderBy(order, "type", b => b.Type);

            return query;
        }
    }
}
