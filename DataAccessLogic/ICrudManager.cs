using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLogic
{
    public enum CouldDelete
    {
        Yes,
        No
    }

    public interface ICrudAsync<EntityT>
    where EntityT : class
    {
        ValueTask<EntityT> CreateAsync(EntityT entity);
        ValueTask<EntityT> GetByIdOrNullAsync(int id);
        void Update(EntityT entity);
        CouldDelete Delete(EntityT entity);
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

        public async ValueTask<EntityT> CreateAsync(EntityT entity)
        {
            await this.DbContext.AddAsync(entity);
            return entity;
        }

        public ValueTask<EntityT> GetByIdOrNullAsync(int id)
        {
            return this.DbContext.FindAsync<EntityT>(id);
        }

        public void Update(EntityT entity)
        {
            this.DbContext.Update(entity);
        }

        public CouldDelete Delete(EntityT entity)
        {
            this.DbContext.Remove(entity);
            return CouldDelete.Yes;
        }
    }
}
