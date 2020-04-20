using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataAccessLogic
{
    class FarmLogger : ILogger
    {
        const int COUNTER_THRESHOLD = 10_000;
        const int MINUTES_BETWEEN_PUSHES = 5;

        readonly string            _categoryName;
        readonly FarmMasterContext _db;
        readonly Counter           _counter;

        public FarmLogger(string categoryName, FarmMasterContext db, Counter counter)
        {
            if(db == null)
                throw new ArgumentNullException(nameof(db), "DI didn't work :(");

            this._categoryName = categoryName;
            this._counter      = counter;
            this._db           = db;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NullScope();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            return;
/*
            if(!this.IsEnabled(logLevel))
                return;

            // BUGFIX: This category is used whenever changes are made to the model.
            //         This class itself makes changes to the model.
            //         Thus, causing an infinite loop, and eventually a stack overflow.
            if(this._categoryName.StartsWith("Microsoft.EntityFrameworkCore"))
                return;

            if(formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            string stateJson = "{ error: \"Could not serialize state.\" }";
            try
            {
                stateJson = JsonSerializer.Serialize(state);
            }
            catch(InvalidOperationException)
            {
                // Getting the weird "Method cannot be called because Type.IsGenericType" error.
            }
            catch(JsonException)
            {
                // Sometimes even a valid state will have a cycle.
            }

            var message = formatter(state, exception);
            var entry = new LogEntry() 
            {
                CategoryName    = this._categoryName,
                DateLogged      = DateTimeOffset.UtcNow,
                EventId         = eventId.Id,
                EventName       = eventId.Name,
                Level           = logLevel,
                StateJson       = stateJson,
                Message         = message
            };

            this.AddEntry(entry);*/
        }

        void AddEntry(LogEntry entry)
        {
            try
            {
                this._db.Add(entry);

                // I have no idea if ASP is multithreaded, and if it is I have no idea if it shares these instances
                // between threads.
                // ... I hope not.
                this._counter.Increment();
                if(this._counter.Count >= COUNTER_THRESHOLD)
                {
                    this._db.SaveChanges();
                    this._counter.Reset();
                }

                if(DateTimeOffset.UtcNow - this._counter.LastReset > TimeSpan.FromMinutes(MINUTES_BETWEEN_PUSHES))
                {
                    this._db.SaveChanges();
                    this._counter.Reset();
                }
            }
            catch(Exception)
            {
                // Edge case where the database doesn't exist yet.
            }
        }
    }

    class NullScope : IDisposable { public void Dispose(){ } }

    // Basically just so we can pass an int by-ref, alongside a DateTime
    class Counter
    {
        public int Count { get; private set; }
        public DateTimeOffset LastReset { get; private set; } = DateTimeOffset.UtcNow;

        public void Increment()

        {
            this.Count++;
        }

        public void Reset()
        {
            this.Count = 0;
            this.LastReset = DateTimeOffset.UtcNow;
        }
    }

    /// <summary>
    /// Provides a logger that will write <see cref="LogEntry"/> to a farm master database.
    /// </summary>
    /// <remarks>
    /// Entries are sent off in a batch, either every 100 entries, or after 5 minutes.
    /// </remarks>
    public class FarmLoggerProvider : ILoggerProvider
    {
        readonly IServiceScope                  _scope;
        readonly FarmMasterContext              _db;
        readonly Counter                        _entryCounter;
        readonly Dictionary<string, FarmLogger> _instances;

        public FarmLoggerProvider(IServiceScopeFactory scopeFactory)
        {
            // The logger should be the only part of the codebase that *creates* new logs, so having a persistant
            // db context shouldn't cause issues (i.e. id conflicts).
            this._scope         = scopeFactory.CreateScope();
            this._db            = this._scope.ServiceProvider.GetRequiredService<FarmMasterContext>();
            this._instances     = new Dictionary<string, FarmLogger>();
            this._entryCounter  = new Counter();
        }

        public ILogger CreateLogger(string categoryName)
        {
            if(!this._instances.ContainsKey(categoryName))
                this._instances[categoryName] = new FarmLogger(categoryName, this._db, this._entryCounter);

            return this._instances[categoryName];
        }

        public void Dispose()
        {
            this._scope.Dispose();
        }
    }
}
