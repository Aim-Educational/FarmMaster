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
                    var contacts = context.HttpContext.RequestServices.GetRequiredService<IServiceContactManager>();
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
                    }
                ),
                resolve: graphql =>
                {
                    // Services
                    var animals = context.HttpContext.RequestServices.GetRequiredService<IServiceAnimalManager>();

                    // Arguments
                    var gender = graphql.GetValueOrNull<Animal.Gender>("gender");
                    var species = graphql.GetValueOrNull<int>("species");

                    // Query forming. Done this weird way to hopefully make EF be efficient with SQL.
                    var query = animals.Query();
                    if(gender != null)
                        query = query.Where(a => a.Sex == gender);
                    if(species != null)
                        query = query.Where(a => a.SpeciesId == species);

                    return query.OrderBy(a => a.Name);
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
            services.AddSingleton<EnumerationGraphType<Animal.Gender>>();
            return services;
        }

        public static T? GetValueOrNull<T>(this ResolveFieldContext<object> graphql, string name) where T : struct
        {
            return (graphql.HasArgument(name))
                ? graphql.GetArgument<T>(name)
                : new T?();
        }
    }
}
