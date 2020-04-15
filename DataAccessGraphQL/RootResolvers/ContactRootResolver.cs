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
            if(order == "id")
                query = query.OrderBy(c => c.ContactId);

            return query;
        }
    }
}
