using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLogic;
using System;
using System.Collections.Generic;
using System.Text;
using DataAccessLogicTests.TestDb;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLogic.Tests
{
    [TestClass()]
    public class DbContextUnitOfWorkTests
    {
        [TestMethod()]
        public void SingleScopeCommitTest()
        {
            var db = UnitTestDbContext.InMemory();
            var uow = new DbContextUnitOfWork<UnitTestDbContext>(db);

            using(var scope = uow.Begin("Single Commit"))
            {
                db.Add(new Product{ Name = "Commitment Ring" });
                Assert.IsTrue(scope.Commit());
            }

            var product = db.Products.First(p => p.Name == "Commitment Ring");
            Assert.IsNotNull(product);
            Assert.AreEqual(EntityState.Unchanged, db.Entry(product).State); // Already saved the Added change, so should be Unchanged now.
        }

        [TestMethod()]
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
                Assert.AreEqual(1, ex.ScopeResults.Count());

                var result = ex.ScopeResults.First();
                Assert.AreEqual(UnitOfWorkScopeState.Rollback, result.ScopeState);
                Assert.AreEqual("Single Rollback", result.ScopeName);
                Assert.IsTrue(result.RollbackReason.Length > 0);
            }
            Assert.IsTrue(thrown);

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
                Assert.AreEqual(1, ex.ScopeResults.Count());

                var result = ex.ScopeResults.First();
                Assert.AreEqual(UnitOfWorkScopeState.None, result.ScopeState);
            }
            Assert.IsTrue(thrown);

            // Test that changes are in fact, not committed
            Assert.AreEqual(0, db.ChangeTracker.Entries().Count());
            Assert.IsNull(db.Products.FirstOrDefault(p => p.Name == "Roll-back"));

            db.SaveChanges();
            Assert.IsNull(db.Products.FirstOrDefault(p => p.Name == "Void"));
        }
    }
}