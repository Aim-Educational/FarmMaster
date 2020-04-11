using DataAccessLogic;
using System;
using System.Collections.Generic;
using System.Text;
using DataAccessLogicTests.TestDb;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

            using(var scope = uow.Begin("Single Commit"))
            {
                db.Add(new Product{ Name = "Commitment Ring" });
                Assert.True(scope.Commit());
            }

            var product = db.Products.First(p => p.Name == "Commitment Ring");
            Assert.NotNull(product);
            Assert.Equal(EntityState.Unchanged, db.Entry(product).State); // Already saved the Added change, so should be Unchanged now.
        }

        [Fact()]
        public void SingleScopeRollbackAndNoneTest()
        {
            var db = UnitTestDbContext.InMemory();
            var uow = new DbContextUnitOfWork<UnitTestDbContext>(db);
            var thrown = false;

            // Test .Rollback
            try
            {
                using (var scope = uow.Begin("Single Rollback"))
                {
                    db.Add(new Product { Name = "Roll-back" });
                    scope.Rollback("Roll-back corp are defunct");
                }
            }
            catch (UnitOfWorkException ex)
            {
                thrown = true;
                Assert.Single(ex.ScopeResults);

                var result = ex.ScopeResults.First();
                Assert.Equal(UnitOfWorkScopeState.Rollback, result.ScopeState);
                Assert.Equal("Single Rollback", result.ScopeName);
                Assert.True(result.RollbackReason.Length > 0);
            }
            Assert.True(thrown);

            // Test not setting a state
            thrown = false;
            try
            {
                using (var scope = uow.Begin("Single None"))
                {
                    db.Add(new Product { Name = "Void" });
                }
            }
            catch (UnitOfWorkException ex)
            {
                thrown = true;
                Assert.Single(ex.ScopeResults);

                var result = ex.ScopeResults.First();
                Assert.Equal(UnitOfWorkScopeState.None, result.ScopeState);
            }
            Assert.True(thrown);

            // Test that changes are in fact, not committed
            Assert.Empty(db.ChangeTracker.Entries());
            Assert.Null(db.Products.FirstOrDefault(p => p.Name == "Roll-back"));

            db.SaveChanges();
            Assert.Null(db.Products.FirstOrDefault(p => p.Name == "Void"));
        }
    }
}