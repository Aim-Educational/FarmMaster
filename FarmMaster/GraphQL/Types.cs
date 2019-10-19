using Business.Model;
using FarmMaster.Services;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.GraphQL
{
    public class ContactGraphType : ObjectGraphType<Contact>
    {
        public ContactGraphType()
        {
            Field(c => c.ContactId, type: typeof(IdGraphType))
                .Name("Id")
                .Description("The contact's ID");
            Field(c => c.FirstNameWithAbbreviatedLastName)
                .Name("Name")
                .Description("The contact's abbreviated name");
        }
    }
}
