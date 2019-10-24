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
                .Description("The contact's ID.");
            Field(c => c.FirstNameWithAbbreviatedLastName)
                .Name("Name")
                .Description("The contact's abbreviated name.");
        }
    }

    public class AnimalGraphType : ObjectGraphType<Animal>
    {
        public AnimalGraphType()
        {
            Field(a => a.AnimalId, type: typeof(IdGraphType))
                .Name("Id")
                .Description("The animal's ID.");
            Field(a => a.Name)
                .Description("The animal's name.");
            Field(a => a.Dad, type: typeof(AnimalGraphType))
                .Description("The animal's dad, if it has one.");
            Field(a => a.Mum, type: typeof(AnimalGraphType))
                .Description("The animal's mum, if it has one.");
            Field("breeds", a => a.Breeds.Select(b => b.Breed.Name))
                .Name("Breeds")
                .Description("The animal's breeds.");
            Field(a => a.Owner, type: typeof(ContactGraphType))
                .Description("The animal's owner.");
            Field(a => a.Sex, type: typeof(EnumerationGraphType<Animal.Gender>))
                .Description("The animal's gender.");
            Field(a => a.Tag)
                .Description("The animal's tag.");
            Field(a => a.Species, type: typeof(SpeciesGraphType))
                .Description("The animal's species.");
        }
    }

    public class SpeciesGraphType : ObjectGraphType<Species>
    {
        public SpeciesGraphType()
        {
            Field(s => s.SpeciesId, type: typeof(IdGraphType))
                .Name("Id")
                .Description("The species' ID.");
            Field(s => s.Name)
                .Description("The species' name.");
        }
    }

    public class BreedGraphType : ObjectGraphType<Breed>
    {
        public BreedGraphType()
        {
            Field(b => b.BreedId, type: typeof(IdGraphType))
                .Name("Id")
                .Description("The breed's ID.");
            Field(b => b.Name)
                .Description("The breed's name.");
        }
    }
}
