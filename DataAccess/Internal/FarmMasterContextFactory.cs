using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace DataAccess.Internal
{
    internal class FarmMasterContextFactory<T> : IDesignTimeDbContextFactory<T>
        where T : DbContext
    {
        private const string CONFIG_PATH = "context_factory.json"; // Relative to ./DataAccess/

        private JsonDocument GetConfig()
        {
            JsonDocument json;

            if(File.Exists(CONFIG_PATH))
                json = JsonDocument.Parse(File.ReadAllText(CONFIG_PATH));
            else
            {
                Console.WriteLine($"No config file detected, you can place your connection strings in '{CONFIG_PATH}' for automated use.");

                json = JsonDocument.Parse("{}");
                File.WriteAllText(CONFIG_PATH, $"{{\n\t\"{typeof(T).Name}\": null\n}}");
            }

            return json;
        }

        public T CreateDbContext(string[] args)
        {
            string connectionString = null;

            // Check the config file for the connection string.
            var config             = this.GetConfig();
            var configHasConString = config.RootElement.TryGetProperty(typeof(T).Name, out JsonElement jsonConnectionString);

            if(configHasConString)
                connectionString = jsonConnectionString.GetString();

            // Fallback to asking the user for input.
            if (connectionString == null)
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
