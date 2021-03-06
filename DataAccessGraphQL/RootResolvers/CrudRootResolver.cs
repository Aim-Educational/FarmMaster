﻿using DataAccessLogic;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccessGraphQL.RootResolvers
{
    /// <summary>
    /// Config for <see cref="CrudRootResolver{TResolveEntity}"/>, passed by the implementing class.
    /// </summary>
    public class CrudRootResolverConfig
    {
        public string ReadPolicy { get; set; }
    }

    /// <summary>
    /// A base class used by any root resolvers that can be accessed via an <see cref="ICrudAsync{EntityT}"/> instance.
    /// </summary>
    public abstract class CrudRootResolver<TResolveEntity> : RootResolver<TResolveEntity>
    where TResolveEntity : class
    {
        protected abstract CrudRootResolverConfig Config { get; }

        readonly ICrudAsync<TResolveEntity> _crud;

        public CrudRootResolver(ICrudAsync<TResolveEntity> crud)
        {
            this._crud = crud;

            // id: ID!
            base.Add(new QueryArgument<NonNullGraphType<IdGraphType>>
            {
                Name = "id",
                Description = "Get by ID"
            });
        }

        protected abstract IQueryable<TResolveEntity> OrderPageQuery(IQueryable<TResolveEntity> query, string order);

        public override Task<object> ResolveAsync(
            IResolveFieldContext<object> context,
            DataAccessUserContext userContext
        )
        {
            return base.ResolveCrudAsync(this.Config.ReadPolicy, this._crud, context, userContext);
        }

        public override Task<IEnumerable<TResolveEntity>> ResolvePageAsync(
            DataAccessUserContext userContext,
            int first,
            int after,
            string order
        )
        {
            // The dynamic ordering is a bit too annoying to express with Managers, so we're accessing .Query
            var query = this._crud.IncludeAll(this._crud.Query());
            query = this.OrderPageQuery(query, order);

            return Task.FromResult(
                query.Skip(after)
                     .Take(first)
                     .AsEnumerable()
            );
        }
    }

    public static class CrudRootResolverHelpers
    {
        public static IQueryable<T> TableOrderBy<T, TKey>(
            this IQueryable<T> query,
            string order,
            string expectedOrder,
            Expression<Func<T, TKey>> keySelector
        )
        where T : class
        {
            if (order == expectedOrder)
                return query.OrderBy(keySelector);
            else if (order == expectedOrder + "_desc")
                return query.OrderByDescending(keySelector);
            else
                return query;
        }

        public static IQueryable<T> TableOrderBy<T>(
            this IQueryable<T> query,
            string order,
            string expectedOrder,
            Expression<Func<T, string>> keySelector
        )
        where T : class
        {
            if(keySelector.Body.NodeType == ExpressionType.MemberAccess)
            {
                var param   = keySelector.Parameters.First();           // entity =>
                var member  = ((MemberExpression)keySelector.Body);     // entity => entity.Key
                var toLower = Expression.Call(member, "ToLower", null); // entity => entity.Key.ToLower()

                keySelector = Expression.Lambda<Func<T, string>>(
                    toLower,
                    param
                );
            }

            return query.TableOrderBy<T, string>(order, expectedOrder, keySelector);
        }
    }
}
