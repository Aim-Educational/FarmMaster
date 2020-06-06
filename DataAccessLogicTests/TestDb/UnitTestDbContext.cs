using Microsoft.EntityFrameworkCore;

namespace DataAccessLogicTests.TestDb
{
    public class UnitTestDbContext : DbContext
    {
        public UnitTestDbContext(DbContextOptions options) : base(options)
        {

        }

        public static UnitTestDbContext InMemory(string name = "test")
        {
            var options = new DbContextOptionsBuilder<UnitTestDbContext>()
                .UseInMemoryDatabase(name)
                .Options;

            return new UnitTestDbContext(options);
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
            .Entity<Product>()
            .HasIndex(p => p.Name)
            .IsUnique();

            modelBuilder
            .Entity<Product>()
            .HasData(
                new Product { ProductId = 1, Name = "Apples" },
                new Product { ProductId = 2, Name = "Tuna" },
                new Product { ProductId = 3, Name = "Bacon" }
            );
        }
    }
}
