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

        public override async Task<object> ResolveAsync(
            IResolveFieldContext<object> context,
            DataAccessUserContext userContext
        )
        {
            await userContext.EnforceHasPolicyAsync(Permissions.Contact.Read);

            // ARGS
            var id = context.GetArgument<int>("id");

            // Find the contact
            var contact = await this._contacts.GetByIdAsync(id);
            if(!contact.Succeeded)
                throw new ExecutionError(contact.GatherErrorMessages().Aggregate((a, b) => $"{a}\n{b}"));

            return contact.Value;
        }

        public override async Task<IEnumerable<Contact>> ResolvePageAsync(
            DataAccessUserContext userContext, 
            int first, 
            int after, 
            string order
        )
        {
            // The dynamic ordering is a bit too annoying to express with Managers, so we're accessing .Query
            var query = this._contacts.Query();
            if(order == "id")
                query = query.OrderBy(c => c.ContactId);

            return query.Skip(after)
                        .Take(first);
        }
    }
}
