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
    public class ContactRootResolver : RootResolver<Contact>
    {
        readonly IContactManager _contacts;

        public ContactRootResolver(IContactManager contacts)
        {
            this._contacts = contacts;

            base.Add(new QueryArgument<NonNullGraphType<IdGraphType>>
            {
                Name = "id",
                Description = "Get a contact by their ID"
            });
        }

        public override Task<object> ResolveAsync(
            IResolveFieldContext<object> context,
            DataAccessUserContext userContext
        )
        {
            return base.ResolveCrudAsync(Permissions.Contact.Read, this._contacts, context, userContext);
        }

        public override Task<IEnumerable<Contact>> ResolvePageAsync(
            DataAccessUserContext userContext, 
            int first, 
            int after, 
            string order
        )
        {
            // The dynamic ordering is a bit too annoying to express with Managers, so we're accessing .Query
            var query = this._contacts.IncludeAll(this._contacts.Query());
            if(order == "id")
                query = query.OrderBy(c => c.ContactId);

            return Task.FromResult(
                        query.Skip(after)
                             .Take(first)
                             .AsEnumerable()
            );
        }
    }
}
