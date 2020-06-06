using DataAccessGraphQL;
using DataAccessGraphQL.Api;
using FarmMaster.Module.Core.Features;
using GraphQL.Server;
using GraphQLModule.Api;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;

namespace GraphQLModule.Features
{
    public class GraphQLConfigureServices : ConfigureServicesPart
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration, ApplicationPartManager appParts)
        {
            var builder = services.AddDataAccessGraphQLSchema()
                                  .AddGraphQL(o =>
                                  {
                                      o.EnableMetrics = false;
                                      o.ExposeExceptions = true;
                                  })
                                  .AddSystemTextJson()
                                  .AddGraphTypes(typeof(DataAccessGraphQLSchema), ServiceLifetime.Scoped);

            this.RegisterModuleGraphQLParts(services, appParts, builder);
        }

        private void RegisterModuleGraphQLParts(IServiceCollection services, ApplicationPartManager appParts, IGraphQLBuilder builder)
        {
            var feature = new GraphQLFeature();
            appParts.PopulateFeature(feature);

            var assemblyHashSet = new HashSet<Assembly>();
            foreach (var part in feature.PartTypes)
            {
                var assembly = part.Assembly;
                if (assemblyHashSet.Add(assembly))
                {
                    builder.AddGraphTypes(assembly, ServiceLifetime.Scoped);
                    services.AddRootResolvers(assembly);
                }

                services.AddScoped(typeof(IGraphQLQueryProvider), part);
                services.AddScoped(typeof(IGraphQLMutationProvider), part);
            }
        }
    }
}
