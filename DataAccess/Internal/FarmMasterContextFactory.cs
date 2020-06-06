using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace DataAccess.Internal
{
    public class FarmMasterContextFactory<T> : IDesignTimeDbContextFactory<T>
        where T : DbContext
    {
        public T CreateDbContext(string[] args)
        {
            // PGSQL doesn't actually need to connect to a database to perform a migration, it just needs a valid connection string.
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseNpgsql("Server=urmum;Database=no;");
            return (T)Activator.CreateInstance(typeof(T), builder.Options);
        }
    }
}
