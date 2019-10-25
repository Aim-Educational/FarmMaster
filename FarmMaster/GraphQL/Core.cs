﻿using Business.Model;
using FarmMaster.Services;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FarmMaster.GraphQL
{
    public class FarmQLSchema : Schema
    {
        public FarmQLSchema(IDependencyResolver resolver) : base(resolver)
        {
            Contract.Requires(resolver != null);
            this.Query = new FarmQLQuery(resolver.Resolve<IHttpContextAccessor>());
        }
    }

    public class FarmQLQuery : ObjectGraphType
    {
        public FarmQLQuery(IHttpContextAccessor context)
        {
            Contract.Requires(context != null);

            Field<ListGraphType<ContactGraphType>>(
                "contacts",
                resolve: _ =>
                {
                    var contacts = context.GetRequiredService<IServiceContactManager>();
                    return contacts.Query().OrderBy(c => c.FullName);
                }
            );
            Field<ListGraphType<AnimalGraphType>>(
                "animals",
                arguments: new QueryArguments(
                    new QueryArgument<EnumerationGraphType<Animal.Gender>>
                    {
                        Name = "gender",
                        Description = "Filter by gender"
                    },
                    new QueryArgument<IdGraphType>
                    {
                        Name = "speciesId",
                        Description = "Filter by species",
                        DefaultValue = null
                    },
                    new QueryArgument<ListGraphType<IdGraphType>>
                    {
                        Name = "breedIds",
                        Description = "Filter by breed(s). Animals must have *any* of the given breeds to be included in the result.",
                        DefaultValue = null
                    },
                    new QueryArgument<StringGraphType>
                    {
                        Name = "nameRegex",
                        Description = "Filter by name using the given regex.",
                        DefaultValue = null
                    },
                    new QueryArgument<IntGraphType>
                    {
                        Name = "skip",
                        Description = "The number of animals to skip over"
                    },
                    new QueryArgument<IntGraphType>
                    {
                        Name = "take",
                        Description = "The number of animals to take"
                    }
                ),
                resolve: graphql =>
                {
                    // Services
                    var animals = context.GetRequiredService<IServiceAnimalManager>();

                    // Arguments
                    var gender = graphql.GetValueOrNull<Animal.Gender>("gender");
                    var species = graphql.GetValueOrNull<int>("speciesId");
                    var take = graphql.GetValueOrNull<int>("take");
                    var skip = graphql.GetValueOrNull<int>("skip");
                    var breeds = graphql.GetArgument<List<int>>("breedIds");
                    var nameRegex = graphql.GetArgument<string>("nameRegex");

                    // Query forming. Done this weird way to hopefully make EF decide to perform the entire query on the database,
                    // instead of using any client-side evaluation.
                    var query = animals.Query();
                    if(gender != null)
                        query = query.Where(a => a.Sex == gender);
                    if(species != null)
                        query = query.Where(a => a.SpeciesId == species);
                    if(breeds != null)
                        query = query.Where(a => a.Breeds.Any(b => breeds.Contains(b.BreedId)));
                    if(nameRegex != null)
                        query = query.Where(a => Regex.IsMatch(a.Name, nameRegex));
                    if (skip != null)
                        query = query.Skip(skip ?? 0);
                    if (take != null)
                        query = query.Take(take ?? 0);

                    return query.Include(a => a.Breeds)
                                 .ThenInclude(b => b.Breed)
                                .Include(a => a.Owner)
                                .Include(a => a.Species)
                                .OrderBy(a => a.Name);
                }
            );
            Field<ListGraphType<SpeciesGraphType>>(
                "species",
                resolve: _ =>
                {
                    var speciesBreeds = context.GetRequiredService<IServiceSpeciesBreedManager>();
                    return speciesBreeds.For<Species>().Query().OrderBy(s => s.Name);
                }
            );
            Field<ListGraphType<BreedGraphType>>(
                "breeds",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> 
                    {
                        Name = "speciesId",
                        Description = "Filter by species"
                    }    
                ),
                resolve: graphql =>
                {
                    // Services
                    var speciesBreeds = context.GetRequiredService<IServiceSpeciesBreedManager>();

                    // Arguments
                    var species = graphql.GetValueOrNull<int>("speciesId");

                    // Query
                    var query = speciesBreeds.For<Breed>().Query();
                    if(species != null)
                        query = query.Where(b => b.SpeciesId == species);

                    return query.OrderBy(b => b.Name);
                }
            );
        }
    }

    public static class Extentions
    {
        public static IServiceCollection AddFarmQLSchema(this IServiceCollection services)
        {
            services.AddSingleton<FarmQLSchema>();
            services.AddSingleton<ContactGraphType>();
            services.AddSingleton<AnimalGraphType>();
            services.AddSingleton<SpeciesGraphType>();
            services.AddSingleton<BreedGraphType>();
            services.AddSingleton<EnumerationGraphType<Animal.Gender>>();
            services.AddSingleton<IntGraphType>();
            return services;
        }

        public static T? GetValueOrNull<T>(this ResolveFieldContext<object> graphql, string name) where T : struct
        {
            return (graphql.HasArgument(name))
                ? graphql.GetArgument<T>(name)
                : new T?();
        }

        internal static T GetRequiredService<T>(this IHttpContextAccessor context) where T : class
        {
            return context.HttpContext.RequestServices.GetRequiredService<T>();
        }
    }
}
