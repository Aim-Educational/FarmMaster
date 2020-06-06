using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DataAccessLogic.Tests
{
    // FarmLogger has a hard dependency on DependencyInjection, hence we need to create a ServiceProvider.
    // You can argue with me all you want about how that's a bad thing, but in FarmLogger's case it saves more
    // headaches than it causes.
    public class FarmLoggerProviderTests
    {
        readonly ServiceCollection _services;

        public FarmLoggerProviderTests()
        {
            var builder = new ServiceCollection();

            builder.AddScoped(_ => FarmMasterContext.InMemory());
            builder.AddSingleton<FarmLoggerProvider>();

            this._services = builder;
        }

        [Fact()]
        public void LoggersShouldBeReused()
        {
            var services = this._services.BuildServiceProvider();
            var logProvider = services.GetRequiredService<FarmLoggerProvider>();

            var loggerA = logProvider.CreateLogger("A");
            var loggerA2 = logProvider.CreateLogger("A");
            var loggerB = logProvider.CreateLogger("B");

            Assert.Same(loggerA, loggerA2);
            Assert.NotSame(loggerA, loggerB);
        }

        [Fact()]
        public void LogsShouldUploadInBatch()
        {
            var services = this._services.BuildServiceProvider();
            var db = services.GetRequiredService<FarmMasterContext>();
            var logProvider = services.GetRequiredService<FarmLoggerProvider>();
            var logger = logProvider.CreateLogger("Batch");

            // NOTE: The flush count is hard coded to 10_000 right now. I know, I know. Bad programmer.
            for (int i = 0; i < 9999; i++)
                logger.Log(LogLevel.Information, "Andy bodged it");
            Assert.Empty(db.LogEntries);

            logger.Log(LogLevel.Information, "Lol");
            //Assert.NotEmpty(db.LogEntries);
        }
    }
}