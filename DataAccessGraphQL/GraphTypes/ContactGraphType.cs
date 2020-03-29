using DataAccess;
using DataAccess.Constants;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace DataAccessGraphQL.GraphTypes
{
    public class ContactGraphType : ObjectGraphType<Contact>
    {
        public ContactGraphType(GraphQLUserContextAccessor contextAccessor)
        {
            var context = contextAccessor.Context;

            Field<StringGraphType>(
                "id",
                resolve: ctx => ctx.Source.ContactId
            );

            FieldAsync<StringGraphType>(
                "name",
                resolve: async ctx => 
                {
                    await context.EnforceHasPolicyAsync(Permissions.Contact.Read);
                    return ctx.Source.Name;
                }
            );
        }
    }
}
