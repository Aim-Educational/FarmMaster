using DataAccess;
using DataAccess.Constants;
using DataAccessGraphQL.RootResolvers;
using DataAccessLogic;
using System.Linq;

namespace CrudModule.GraphQL.Resolvers
{
    public class ContactRootResolver : CrudRootResolver<Contact>
    {
        static readonly CrudRootResolverConfig CONFIG_INSTANCE = new CrudRootResolverConfig
        {
            ReadPolicy = Permissions.Contact.Read
        };
        protected override CrudRootResolverConfig Config => CONFIG_INSTANCE;

        public ContactRootResolver(IContactManager contacts) : base(contacts)
        {
        }

        protected override IQueryable<Contact> OrderPageQuery(IQueryable<Contact> query, string order)
        {
            if (order == "id")
                query = query.OrderBy(c => c.ContactId);

            return query;
        }
    }
}
