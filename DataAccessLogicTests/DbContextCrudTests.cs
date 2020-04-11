using DataAccess;
using DataAccessLogic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DataAccessLogicTests
{
    public class DbContextCrudTests
    {
        [Fact()]
        public async Task BasicCrud()
        {
            var db      = FarmMasterContext.InMemory();
            var species = new SpeciesManager(db);
            var uow     = new DbContextUnitOfWork<FarmMasterContext>(db);

            var goat = new Species
            {
                GestrationPeriod = TimeSpan.FromDays(121),
                Name             = "Goat"
            };

            // C & R (DbSet)
            using(var scope = uow.Begin())
            {
                var result = await species.CreateAsync(goat);
                Assert.True(result.Succeeded);
                scope.Commit();
            }
            Assert.NotEmpty(db.Species);
            Assert.Same(goat, db.Species.First());

            // U & R (Query)
            using(var scope = uow.Begin())
            {
                goat.GestrationPeriod = TimeSpan.FromDays(1);
                species.Update(goat);
                scope.Commit();
            }
            Assert.Equal(TimeSpan.FromDays(1), species.Query().AsNoTracking().First().GestrationPeriod);

            // D & R (ById exist and not exist)
            var id = goat.SpeciesId;
            var valueResult = await species.GetByIdAsync(id);
            Assert.True(valueResult.Succeeded);
            Assert.Same(goat, valueResult.Value);

            using(var scope = uow.Begin())
            {
                species.Delete(goat);
                scope.Commit();
            }
            Assert.Empty(db.Species);

            valueResult = await species.GetByIdAsync(id);
            Assert.False(valueResult.Succeeded);
            Assert.Contains("1", valueResult.GatherErrorMessages().First());
        }
    }
}
