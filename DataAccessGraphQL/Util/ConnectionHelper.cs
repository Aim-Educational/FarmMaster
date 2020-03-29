using GraphQL.Builders;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using DataAccessGraphQL.Constants;
using GraphQL.Types.Relay.DataObjects;
using GraphQL;

namespace DataAccessGraphQL.Util
{
    internal static class ConnectionHelper
    {
        public delegate Task<IEnumerable<TSourceType>> ConnectionNodeResolver<TSourceType>(
            DataAccessUserContext ctx, 
            int first, 
            int after,
            string order
        );

        public static void DefineConnectionAsync<TMyT, TGraphType, TSourceType>(
            this ObjectGraphType<TMyT> obj, 
            string name,
            DataAccessUserContext userContext,
            ConnectionNodeResolver<TSourceType> nodeResolve
        ) where TGraphType : IGraphType
        {
            obj.Connection<TGraphType>()
                .Name(name)
                .Argument<IntGraphType>("first", "Only get the first n elements")
                .Argument<IntGraphType>("after", "Skip elements up to the 'after' element")
                .Argument<StringGraphType>("order", "Specifies how to order the data. This is type-specific, but you can generally just type the name of a field.")
                .ResolveAsync(async ctx =>
                {
                    var first = ctx.GetArgument<int>("first");
                    var after = ctx.GetArgument<int>("after");
                    var order = ctx.GetArgument<string>("order");

                    if(first >= PagingConstants.ItemsPerPage || first == 0)
                        first = PagingConstants.ItemsPerPage;

                    var values = await nodeResolve(userContext, first, after, order);

                    return new Connection<TSourceType>
                    {
                        TotalCount = values.Count(),
                        PageInfo   = new PageInfo
                        {
                            EndCursor       = Convert.ToString(after + values.Count()),
                            StartCursor     = Convert.ToString(after),
                            HasNextPage     = values.Count() == PagingConstants.ItemsPerPage,
                            HasPreviousPage = after > 0
                        },
                        Edges = values.Select(c => new Edge<TSourceType>
                        {
                            Cursor = "TODO",
                            Node   = c
                        }).ToList()
                    };
                });
        }
    }
}
