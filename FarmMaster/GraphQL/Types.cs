﻿using Business.Model;
using FarmMaster.Services;
using GraphQL;
using GraphQL.Types;
using GroupScript;
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

    public class HoldingGraphType : ObjectGraphType<Holding>
    {
        public HoldingGraphType()
        {
            Field(h => h.HoldingId, type: typeof(IdGraphType))
                .Name("Id")
                .Description("The holdings's ID.");
            Field(h => h.Name)
                .Name("Name")
                .Description("The holding's abbreviated name.");
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
            Field("breeds", a => a.Breeds.Select(b => b.Breed.Name) ?? null)
                .Description("The animal's breeds.");
            Field(a => a.Owner, type: typeof(ContactGraphType))
                .Description("The animal's owner.");
            Field(a => a.Sex, type: typeof(EnumerationGraphType<Animal.Gender>))
                .Description("The animal's gender.");
            Field(a => a.Tag)
                .Description("The animal's tag.");
            Field(a => a.Species, type: typeof(SpeciesGraphType))
                .Description("The animal's species.");
            Field("imageId", a => a.ImageId ?? -1)
                .Description("The Id for the animal's image, used with the /Image/Get API.");
            Field("lifeEventEntries", 
                  a => a.LifeEventEntries.Select(m => m.LifeEventEntry), 
                  type: typeof(ListGraphType<LifeEventEntryGraphType>))
                .Description("The entries for all of the animal's life events.");
            Field(a => a.IsEndOfSystem)
                .Description("Whether or not the animal is marked as 'end of system'. a.k.a dead, sold, archived, etc.");
            Field("groups",
                  a => a.Groups.Select(m => m.AnimalGroup),
                  type: typeof(ListGraphType<AnimalGroupGraphType>))
                .Description("All of the animal groups that this animal belongs to.");
        }
    }

    public class LifeEventGraphType : ObjectGraphType<LifeEvent>
    {
        public LifeEventGraphType()
        {
            Field(e => e.LifeEventId, type: typeof(IdGraphType))
                .Name("Id")
                .Description("The ID for the life event.");
            Field(e => e.Name)
                .Description("The life event's name.");
        }
    }

    public class LifeEventEntryGraphType : ObjectGraphType<LifeEventEntry>
    {
        public LifeEventEntryGraphType()
        {
            Field(e => e.LifeEventEntryId, type: typeof(IdGraphType))
                .Name("Id")
                .Description("The ID for the entry.");
            Field(e => e.DateTimeCreated)
                .Name("DateTimeUtc")
                .Description("The UTC time that the entry was created.");
            Field(e => e.LifeEvent, type: typeof(LifeEventGraphType))
                .Description("The life event this entry belongs to.");
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

    public class AnimalGroupGraphType : ObjectGraphType<AnimalGroup>
    {
        public AnimalGroupGraphType()
        {
            Field(g => g.AnimalGroupId, type: typeof(IdGraphType))
                .Name("Id")
                .Description("The group's ID.");
            Field(g => g.Description)
                .Name("Description")
                .Description("The group's description.");
            Field(g => g.Name)
                .Name("Name")
                .Description("The group's name.");
        }
    }

    public class AnimalGroupScriptGraphType : ObjectGraphType<AnimalGroupScript>
    {
        public AnimalGroupScriptGraphType()
        {
            Field(s => s.AnimalGroupScriptId, type: typeof(IdGraphType))
                .Name("Id")
                .Description("The scripts's ID.");
            Field(s => s.Name)
                .Name("Name")
                .Description("The script's name.");
            Field("parameters",
                  s => 
                      (new GroupScriptNodeTree(new GroupScriptParser(s.Code))).Parameters.Select(p => new AnimalGroupScriptParameter
                      {
                          Name = p.Name,
                          TypeName = Convert.ToString(p.DataType) // Very. VERY. temporary
                      }),
                  type: typeof(ListGraphType<AnimalGroupScriptParameterGraphType>)
            );
            Field(s => s.Code)
                .Name("Code")
                .Description("The script's code.");
        }
    }

    // Don't think this should really be in this file, but it'll live here for a bit.
    public class AnimalGroupScriptParameter
    {
        public string Name { get; set; }
        public string TypeName { get; set; } // TODO: Turn this into an enum at some point :P
    }

    public class AnimalGroupScriptParameterGraphType : ObjectGraphType<AnimalGroupScriptParameter>
    {
        public AnimalGroupScriptParameterGraphType()
        {
            Field(p => p.Name)
                .Name("Name")
                .Description("The parameter's name.");
            Field(p => p.TypeName)
                .Name("TypeName")
                .Description("The name of the parameter's type.");
        }
    }
}
