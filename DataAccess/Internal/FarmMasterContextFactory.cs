using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Internal
{
    internal class FarmMasterContextFactory<T> : IDesignTimeDbContextFactory<T>
        where T : DbContext
    {
        public T CreateDbContext(string[] args)
        {
            string connectionString = null;
            if (connectionString == null) // Supporting multiple options in the future, which is why this check seems weird on its own.
            {
                Console.Write("Connection String: ");
                connectionString = Console.ReadLine();
            }

            var builder = new DbContextOptionsBuilder<T>();
            builder.UseNpgsql(connectionString);

            return (T)Activator.CreateInstance(typeof(T), builder.Options);
        }
    }
}
