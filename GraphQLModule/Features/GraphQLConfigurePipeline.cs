using DataAccessGraphQL;
using FarmMaster.Module.Core.Features;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQLModule.Features
{
    public class GraphQLConfigurePipeline : ConfigurePipelinePart
    {
        public override void Configure(IApplicationBuilder app, IServiceProvider services, IWebHostEnvironment env)
        {
            app.UseGraphQL<DataAccessGraphQLSchema>("/graphql");
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions
            {
                PlaygroundSettings = new Dictionary<string, object> {
                    { "request.credentials", "same-origin" } // Instructs the playground to send cookies, so we can validate the user.
                }
            });
        }
    }
}
