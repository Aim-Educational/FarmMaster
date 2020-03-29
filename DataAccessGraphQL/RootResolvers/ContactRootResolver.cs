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
    public class ContactRootResolver : RootResolver
    {
        public IContactManager Manager { get; private set; }

        public ContactRootResolver(IContactManager contacts)
        {
            this.Manager = contacts;

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
            var contact = await this.Manager.GetByIdAsync(id);
            if(contact == null)
                throw new ExecutionError($"No contact with ID #{id} was found");

            return contact;
        }
    }
}
