using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccessLogic
{
    public interface ICrudAsync<EntityT>
    where EntityT : class
    {
        Task<ValueResultObject<EntityT>> CreateAsync(EntityT entity);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Implementors should implicitly use <see cref="IncludeAll(IQueryable{EntityT})"/> for this function.
        /// 
        /// Either provide a specialised function(s), or direct the user to <see cref="Query"/> if the performance
        /// hit is too much for certain cases.
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ValueResultObject<EntityT>> GetByIdAsync(int id);
        ResultObject Update(EntityT entity);
        ResultObject Delete(EntityT entity);

        /// <summary>
        /// Last resort/optimised read-only function.
        /// </summary>
        /// <returns></returns>
        IQueryable<EntityT> Query();

        /// <summary>
        /// Ditto
        /// </summary>
        /// <returns></returns>
        IQueryable<EntityT> IncludeAll(IQueryable<EntityT> query);
    }

    public abstract class DbContextCrud<EntityT, DatabaseT> : ICrudAsync<EntityT>
    where EntityT : class
    where DatabaseT : DbContext
    {
        protected DatabaseT DbContext { get; private set; }

        public DbContextCrud(DatabaseT db)
        {
            if (db == null)
                throw new ArgumentNullException(nameof(db));

            this.DbContext = db;
        }

        public async Task<ValueResultObject<EntityT>> CreateAsync(EntityT entity)
        {
            await this.DbContext.AddAsync(entity);
            return new ValueResultObject<EntityT>()
            {
                Succeeded = true,
                Value = entity
            };
        }

        public async Task<ValueResultObject<EntityT>> GetByIdAsync(int id)
        {
            // Find field marked [Key]
            var keyProp = typeof(EntityT).GetProperties()
                                         .Single(p => p.IsDefined(typeof(KeyAttribute), true));

            // Dynamically create an Expression to see if the key == id
            var parameter = Expression.Parameter(typeof(EntityT), "e"); // (EntityT e) =>
            var property = Expression.Property(parameter, keyProp);    // e => e.Id
            var constant = Expression.Constant(id);                    // id
            var equals = Expression.Equal(property, constant);       // e => e.Id == id
            var expression = Expression.Lambda<Func<EntityT, bool>>(equals, parameter);

            var entity = await this.IncludeAll(this.Query())
                                   .SingleOrDefaultAsync(expression);
            return (entity == null)
                ? new ValueResultObject<EntityT>()
                {
                    Succeeded = false,
                    Errors = new List<string>() { $"No {typeof(EntityT).Name} with ID of #{id} was found." }
                }
                : new ValueResultObject<EntityT>()
                {
                    Succeeded = true,
                    Value = entity
                };
        }

        public ResultObject Update(EntityT entity)
        {
            this.DbContext.Update(entity);
            return ResultObject.Ok;
        }

        public ResultObject Delete(EntityT entity)
        {
            this.DbContext.Remove(entity);
            return ResultObject.Ok;
        }

        public IQueryable<EntityT> Query()
        {
            return this.DbContext.Set<EntityT>();
        }

        public virtual IQueryable<EntityT> IncludeAll(IQueryable<EntityT> query)
        {
            return query;
        }
    }
}
