using DataAccess;
using DataAccessLogic;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public abstract class RootResolver<TResolveEntity> : QueryArguments
    where TResolveEntity : class
    {
        // This has to return Task<object> due to GraphQL.NET wanting it.
        public abstract Task<object> ResolveAsync(
            IResolveFieldContext<object> context,
            DataAccessUserContext userContext
        );

        public abstract Task<IEnumerable<TResolveEntity>> ResolvePageAsync(
            DataAccessUserContext userContext,
            int first, 
            int after, 
            string order
        );

        /// <summary>
        /// This is a helper method for resolvers to resolve an entity via an <see cref="ICrudAsync"/>
        /// </summary>
        /// <param name="context">The GraphQL context. Used to access parameters.</param>
        /// <param name="crud">The <see cref="ICrudAsync{EntityT}"/> used to resolve the entity.</param>
        /// <param name="idParamName">The name of the GraphQL parameter containing the entity's ID. Defaults to 'id'.</param>
        /// <param name="policy">The ASP Core policy that the user must pass in order to access this function.</param>
        /// <param name="userContext">Used to gain access to the user performing this action.</param>
        protected async Task<object> ResolveCrudAsync(
            string policy,
            ICrudAsync<TResolveEntity> crud,
            IResolveFieldContext<object> context,
            DataAccessUserContext userContext,
            string idParamName = "id"
        )
        {
            await userContext.EnforceHasPolicyAsync(policy);

            // ARGS
            var id = context.GetArgument<int>(idParamName);

            // Find the species
            var entity = await crud.GetByIdAsync(id);
            if (!entity.Succeeded)
                throw new ExecutionError(entity.GatherErrorMessages().Aggregate((a, b) => $"{a}\n{b}"));

            return entity.Value;
        }
    }
}
