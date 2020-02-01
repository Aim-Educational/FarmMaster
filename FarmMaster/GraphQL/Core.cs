using Business.Model;
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
            Field<ListGraphType<HoldingGraphType>>(
                "holdings",
                resolve: _ =>
                {
                    var holdings = context.GetRequiredService<IServiceHoldingManager>();
                    return holdings.Query().OrderBy(h => h.Name);
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
                        Description = "Filter by breed(s). Animals must have *any* of the given breeds to be included in the result",
                        DefaultValue = null
                    },
                    new QueryArgument<IdGraphType>
                    {
                        Name = "id",
                        Description = "Filter by a specific animal's Id",
                        DefaultValue = null
                    },
                    new QueryArgument<StringGraphType>
                    {
                        Name = "nameRegex",
                        Description = "Filter by name using the given regex",
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
                    },
                    new QueryArgument<BooleanGraphType>
                    {
                        Name = "isEndOfSystem",
                        Description = "Filter by whether an animal is marked 'end of system'."
                    },
                    new QueryArgument<ListGraphType<IdGraphType>> 
                    {
                        Name = "groupIds",
                        Description = "Filter by whether the animal is included in *any* of these animal groups."
                    }
                ),
                resolve: graphql =>
                {
                    // Services
                    var animals = context.GetRequiredService<IServiceAnimalManager>();
                    var db      = context.GetRequiredService<FarmMasterContext>();

                    // Arguments
                    var gender      = graphql.GetValueOrNull<Animal.Gender>("gender");
                    var species     = graphql.GetValueOrNull<int>("speciesId");
                    var take        = graphql.GetValueOrNull<int>("take");
                    var skip        = graphql.GetValueOrNull<int>("skip");
                    var breeds      = graphql.GetArgument<List<int>>("breedIds");
                    var nameRegex   = graphql.GetArgument<string>("nameRegex");
                    var animalId    = graphql.GetValueOrNull<int>("id");
                    var endOfSystem = graphql.GetValueOrNull<bool>("isEndOfSystem");
                    var groups      = graphql.GetArgument<List<int>>("groupIds");

                    return animals.Query()
                                  .Include(a => a.Breeds)
                                   .ThenInclude(b => b.Breed)
                                  .Include(a => a.Owner)
                                  .Include(a => a.Species)
                                  .Include(a => a.LifeEventEntries)
                                   .ThenInclude(m => m.LifeEventEntry)
                                    .ThenInclude(e => e.LifeEvent)
                                  .Include(a => a.Groups)
                                   .ThenInclude(m => m.AnimalGroup)
                                  .Where(a => gender      == null || a.Sex == gender)
                                  .Where(a => species     == null || a.SpeciesId == species)
                                  .Where(a => breeds      == null || a.Breeds.Any(b => breeds.Contains(b.BreedId)))
                                  .Where(a => nameRegex   == null || Regex.IsMatch(a.Name, nameRegex))
                                  .Where(a => animalId    == null || a.AnimalId == animalId)
                                  .Where(a => endOfSystem == null || a.LifeEventEntries.Any(m => m.LifeEventEntry.LifeEvent.IsEndOfSystem) == endOfSystem) // Because the normal "Animal.IsEndOfSystem" is client-side, which we want to avoid
                                  .Where(a => groups      == null || a.Groups.Any(m => groups.Contains(m.AnimalGroupId)))
                                  .OrderBy(a => a.AnimalId) // Weird edge case.
                                  .Skip(skip ?? 0)
                                  .Take(take ?? int.MaxValue)
                                  .OrderBy(a => a.Name);
                }
            );
            Field<ListGraphType<LifeEventGraphType>>(
                "lifeEvents",
                arguments: new QueryArguments(
                    new QueryArgument<EnumerationGraphType<LifeEvent.TargetType>>
                    {
                        Name = "target",
                        Description = "Filter by target"
                    }
                ),
                resolve: graphql =>
                {
                    var target = graphql.GetValueOrNull<LifeEvent.TargetType>("target");

                    var lifeEvents = context.GetRequiredService<IServiceLifeEventManager>();
                    return lifeEvents.For<LifeEvent>()
                                     .Query()
                                     .Where(e => target == null || e.Target == target)
                                     .OrderBy(e => e.Name);
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

                    return speciesBreeds.For<Breed>()
                                        .Query()
                                        .Where(b => species == null || b.SpeciesId == species)
                                        .OrderBy(b => b.Name);
                }
            ); 
            Field<ListGraphType<AnimalGroupGraphType>>(
                "animalGroups",
                arguments: new QueryArguments(
                new QueryArgument<IdGraphType>
                {
                    Name = "hasAnimalId",
                    Description = "Whether the group has the specific animal assigned to it.",
                    DefaultValue = null
                },
                new QueryArgument<IdGraphType>
                {
                    Name = "hasNotAnimalId",
                    Description = "Whether the group does not have the specific animal assigned to it.",
                    DefaultValue = null
                },
                new QueryArgument<IdGraphType>
                {
                    Name = "id",
                    Description = "Filter by a specific animal group's ID",
                    DefaultValue = null
                }),
                resolve: graphql =>
                {
                    // Arguments
                    var hasAnimalId    = graphql.GetValueOrNull<int>("hasAnimalId");
                    var hasNotAnimalId = graphql.GetValueOrNull<int>("hasNotAnimalId");
                    var id             = graphql.GetValueOrNull<int>("id");

                    var groups = context.GetRequiredService<IServiceAnimalGroupManager>();
                    return groups.Query()
                                 .Include(g => g.Animals)
                                 .Include(g => g.AutomatedScripts)
                                  .ThenInclude(g => g.AnimalGroupScript)
                                 .Where(g => hasAnimalId    == null || g.Animals.Any(m => m.AnimalId == hasAnimalId))
                                 .Where(g => hasNotAnimalId == null || g.Animals.All(m => m.AnimalId != hasNotAnimalId))
                                 .Where(g => id             == null || g.AnimalGroupId == id)
                                 .OrderBy(g => g.Name);
                }
            );
            Field<ListGraphType<AnimalGroupScriptGraphType>>(
                "animalGroupScripts",
                arguments: new QueryArguments(
                new QueryArgument<IdGraphType>
                {
                    Name = "name",
                    Description = "Filter by a specific script name.",
                    DefaultValue = null
                }),
                resolve: graphql => 
                { 
                    // Arguments
                    var name = graphql.GetArgument<string>("name");

                    var scripts = context.GetRequiredService<IServiceAnimalGroupScriptManager>();

                    return scripts.Query()
                                  .Where(s => name == null || s.Name == name)
                                  .OrderBy(s => s.Name);
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
            services.AddSingleton<HoldingGraphType>();
            services.AddSingleton<LifeEventGraphType>();
            services.AddSingleton<LifeEventEntryGraphType>();
            services.AddSingleton<AnimalGroupGraphType>();
            services.AddSingleton<AnimalGroupScriptGraphType>();
            services.AddSingleton<AnimalGroupScriptParameterGraphType>();
            services.AddSingleton<AnimalGroupScriptAutoScriptGraphType>();
            services.AddSingleton<ListGraphType<LifeEventEntryGraphType>>();
            services.AddSingleton<ListGraphType<AnimalGroupScriptParameterGraphType>>();
            services.AddSingleton<EnumerationGraphType<Animal.Gender>>();
            services.AddSingleton<EnumerationGraphType<LifeEvent.TargetType>>();
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
