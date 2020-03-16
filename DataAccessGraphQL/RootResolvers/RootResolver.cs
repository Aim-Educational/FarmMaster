using DataAccess;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessGraphQL.RootResolvers
{
    public abstract class RootResolver : QueryArguments
    {
        public abstract Task<object> ResolveAsync(
            IResolveFieldContext<object> context,
            DataAccessUserContext userContext
        );
    }
}
