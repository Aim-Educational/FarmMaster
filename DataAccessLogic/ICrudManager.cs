using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLogic
{
    public interface ICrudAsync<EntityT>
    where EntityT : class
    {
        Task<ValueResultObject<EntityT>> CreateAsync(EntityT entity);
        Task<ValueResultObject<EntityT>> GetByIdAsync(int id);
        ResultObject Update(EntityT entity);
        ResultObject Delete(EntityT entity);

        /// <summary>
        /// Last resort/optimised read-only function.
        /// </summary>
        /// <returns></returns>
        IQueryable<EntityT> Query();
    }

    public abstract class DbContextCrud<EntityT, DatabaseT> : ICrudAsync<EntityT>
    where EntityT : class
    where DatabaseT : DbContext
    {
        protected DbContext DbContext { get; private set; } 

        public DbContextCrud(DatabaseT db)
        {
            if(db == null)
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
            var entity = await this.DbContext.FindAsync<EntityT>(id);
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
            return ResultObject.Ok();
        }

        public ResultObject Delete(EntityT entity)
        {
            this.DbContext.Remove(entity);
            return ResultObject.Ok();
        }

        public IQueryable<EntityT> Query()
        {
            return this.DbContext.Set<EntityT>();
        }
    }
}
