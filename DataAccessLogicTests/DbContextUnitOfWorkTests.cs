using DataAccessLogicTests.TestDb;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

namespace DataAccessLogic.Tests
{
    public class DbContextUnitOfWorkTests
    {
        [Fact()]
        public void SingleScopeCommitTest()
        {
            var db = UnitTestDbContext.InMemory();
            var uow = new DbContextUnitOfWork<UnitTestDbContext>(db);

            UnitOfWorkScope scope = null;
            using (scope = uow.Begin("Single Commit"))
            {
                db.Add(new Product { Name = "Commitment Ring" });
                Assert.True(scope.Commit());
            }

            Assert.Equal(UnitOfWorkScopeState.Commit, scope.State);

            var product = db.Products.First(p => p.Name == "Commitment Ring");
            Assert.NotNull(product);
            Assert.Equal(EntityState.Unchanged, db.Entry(product).State); // Already saved the Added change, so should be Unchanged now.
        }

        [Fact()]
        public void SingleScopeRollbackAndNoneTest()
        {
            var db = UnitTestDbContext.InMemory();
            var uow = new DbContextUnitOfWork<UnitTestDbContext>(db);

            // Test .Rollback
            UnitOfWorkScope scope;
            using (scope = uow.Begin("Single Rollback"))
            {
                db.Add(new Product { Name = "Roll-back" });
                scope.Rollback("Roll-back corp are defunct");
            }

            Assert.Equal(UnitOfWorkScopeState.Rollback, scope.State);
            Assert.Equal("Single Rollback", scope.Name);
            Assert.True(scope.RollbackReason.Length > 0);

            // Test not setting a state
            using (scope = uow.Begin("Single None"))
            {
                db.Add(new Product { Name = "Void" });
            }

            Assert.Equal(UnitOfWorkScopeState.None, scope.State);

            // Test that changes are in fact, not committed
            Assert.Empty(db.ChangeTracker.Entries());
            Assert.Null(db.Products.FirstOrDefault(p => p.Name == "Roll-back"));

            db.SaveChanges();
            Assert.Null(db.Products.FirstOrDefault(p => p.Name == "Void"));
        }
    }
}