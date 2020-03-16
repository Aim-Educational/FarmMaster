﻿using DataAccess;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessGraphQL.RootResolvers
{
    /// <summary>
    /// Used for resolvers shared between classes.
    /// </summary>
    /// <remarks>
    /// Example:
    /// 
    /// Both RootQuery and RootMutation provide the `user` field, so therefor share logic.
    /// 
    /// To make this logic bundled up into a single class, the `UserRootResolver` is created, containing
    /// the arguments needed, as well as the actual function to resolve the type.
    /// </remarks>
    public abstract class RootResolver : QueryArguments
    {
        public abstract Task<object> ResolveAsync(
            IResolveFieldContext<object> context,
            DataAccessUserContext userContext
        );
    }
}
