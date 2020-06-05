using FarmMaster.Module.Core.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.SystemTextJson;
using DataAccessGraphQL;

namespace GraphQLModule.Features
{
    public class GraphQLConfigureServices : ConfigureServicesPart
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataAccessGraphQLSchema();
            services.AddGraphQL(o =>
            {
                o.EnableMetrics = false;
                o.ExposeExceptions = true;
            })
            .AddSystemTextJson()
            .AddGraphTypes(typeof(DataAccessGraphQLSchema), ServiceLifetime.Scoped);
        }
    }
}
