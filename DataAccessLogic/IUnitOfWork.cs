using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLogic
{
    /// <summary>
    /// Describes the state of a <see cref="UnitOfWorkScope"/>
    /// </summary>
    public enum UnitOfWorkScopeState
    {
        /// <summary>
        /// No state has been set yet.
        /// </summary>
        None,

        /// <summary>
        /// The scope's changes should be committed.
        /// </summary>
        Commit,

        /// <summary>
        /// The scope's changes shouldn't be used/rollled back.
        /// </summary>
        Rollback
    }

    /// <summary>
    /// A singular unit of work. For ASP Core, there will generally be unit of work per request (scoped service).
    /// </summary>
    /// <remarks>
    /// This assembly (<see cref="DataAccessLogic"/>) works primarily with a <see cref="DbContext"/> by default.
    /// 
    /// Multiple scopes can be used at once. When more than one scope is active, this is called a "unit of work chain".
    /// 
    /// *All* scopes in a chain must end with the <see cref="UnitOfWorkScopeState.Commit"/> state, otherwise all changes
    /// from the chain are rejected, and a <see cref="UnitOfWorkException"/> is thrown.
    /// 
    /// After all scopes are .Dispose() of, and only if all scopes end with the <see cref="UnitOfWorkScopeState.Commit"/>
    /// state, then all changes are committed.
    /// 
    /// Scopes can be given names, so in the even that a <see cref="UnitOfWorkException"/> is thrown, the user
    /// can inspect the <see cref="UnitOfWorkException.ScopeResults"/> field to see which scopes failed, helpful for debugging.
    /// </remarks>
    public interface IUnitOfWork
    {
        UnitOfWorkScope Begin(string scopeName = null);
        void OnScopeDisposing(UnitOfWorkScope scope);
    }

    /// <summary>
    /// Contains information about a unit of work scope.
    /// </summary>
    public sealed class UnitOfWorkScope : IDisposable
    {
        bool                 _isDisposed;
        readonly IUnitOfWork _owner;

        /// <summary>
        /// The name of this scope.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The state of this scope.
        /// </summary>
        public UnitOfWorkScopeState State { get; private set; }

        /// <summary>
        /// The reason this scope was rolledback, if it even was in the first place.
        /// </summary>
        public string RollbackReason { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of this scope - used for debugging purposes.</param>
        /// <param name="owner">The owner of this scope.</param>
        public UnitOfWorkScope(string name, IUnitOfWork owner)
        {
            if(owner == null)
                throw new ArgumentNullException(nameof(owner));

            this._owner = owner;
            this.Name   = name;
        }

        /// <summary>
        /// Mark this scope as a success.
        /// </summary>
        /// <returns>True if this scope could be marked as 'commit', false otherwise (e.g. it's already been set to 'rollback')</returns>
        public bool Commit()
        {
            return this.ChangeState(UnitOfWorkScopeState.Commit);
        }

        /// <summary>
        /// Mark this scope as a failure.
        /// </summary>
        /// <param name="reason">The reason this scope was rolled back, used for debugging.</param>
        /// <returns>True if this scope could be marked as 'rollback', false otherwise (e.g. it's already been set to 'commit')</returns>
        public bool Rollback(string reason)
        {
            this.RollbackReason = reason;
            return this.ChangeState(UnitOfWorkScopeState.Rollback);
        }

        /// <summary>
        /// Disposes of this scope, calling the <see cref="IUnitOfWork.OnScopeDisposing(UnitOfWorkScope, UnitOfWorkScopeState)"/>
        /// function of this scope's owner.
        /// </summary>
        public void Dispose()
        {
            if(this._isDisposed)
                return;

            this._isDisposed = true;
            this._owner.OnScopeDisposing(this);
        }

        private bool ChangeState(UnitOfWorkScopeState state)
        {
            this.EnforceNotDisposed();
            if(this.State != UnitOfWorkScopeState.None)
                return false;

            this.State = state;
            return true;
        }

        private void EnforceNotDisposed()
        {
            if(this._isDisposed)
                throw new ObjectDisposedException(nameof(UnitOfWorkScope));
        }
    }

    /// <summary>
    /// The default implementation of <see cref="IUnitOfWork"/> which works off of a <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="ContextT">The database type to use</typeparam>
    /// <remarks>
    /// If a unit of work chain is rejected, *all* unsaved changes to the database are discarded, even those performed
    /// outside of a unit of work scope. Don't mix manual .SaveChanges with the unit of work pattern.
    /// </remarks>
    public sealed class DbContextUnitOfWork<ContextT> : IUnitOfWork
    where ContextT : DbContext
    {
        int                     _activeScopeCount;
        IList<UnitOfWorkResult> _results;
        readonly ContextT       _db;

        public DbContextUnitOfWork(ContextT db)
        {
            this._db = db;
            this._results = new List<UnitOfWorkResult>();
        }

        public UnitOfWorkScope Begin(string scopeName = null)
        {
            this._activeScopeCount++;
            return new UnitOfWorkScope(scopeName, this);
        }

        public void OnScopeDisposing(UnitOfWorkScope scope)
        {
            if(scope == null)
                throw new ArgumentNullException(nameof(scope));

            this._activeScopeCount--;
            this._results.Add(new UnitOfWorkResult(scope.Name, scope.State, scope.RollbackReason));

            if(this._activeScopeCount == 0)
                this.OnAllScopesDisposed();
        }

        private void OnAllScopesDisposed()
        {
            if(this._results.Any(r => r.ScopeState != UnitOfWorkScopeState.Commit))
            {
                this.RollbackDatabase();

                var results = this._results;
                this._results = new List<UnitOfWorkResult>(); // We need to clear the list, but the exception needs the data, so we new() instead of .Clear().
                throw new UnitOfWorkException(results);
            }

            this._results.Clear();
            this._db.SaveChanges();
        }

        private void RollbackDatabase()
        {
            var changes = this._db.ChangeTracker
                                  .Entries()
                                  .Where(x => x.State != EntityState.Unchanged)
                                  .ToList(); // Since we'll be modifying the collection as we iterate it.

            foreach(var change in changes)
            {
                switch(change.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        change.State = EntityState.Unchanged;
                        break;

                    case EntityState.Added:
                        change.State = EntityState.Detached;
                        break;

                    default: break;
                }
            }
        }
    }

    public sealed class UnitOfWorkResult
    {
        public string ScopeName { get; private set; }
        public UnitOfWorkScopeState ScopeState { get; private set; }
        public string RollbackReason { get; private set; }

        public UnitOfWorkResult(string scopeName, UnitOfWorkScopeState state, string rollbackReason)
        {
            this.ScopeName = scopeName;
            this.ScopeState = state;
            this.RollbackReason = rollbackReason;
        }

        public override string ToString()
        {
            var output = $"[{this.ScopeName}] - {this.ScopeState}";

            return (this.ScopeState == UnitOfWorkScopeState.Rollback)
                ? $"{output}: {this.RollbackReason}"
                : output;
        }
    }

    public class UnitOfWorkException : Exception
    {
        public IEnumerable<UnitOfWorkResult> ScopeResults { get; private set; }

        public UnitOfWorkException(IEnumerable<UnitOfWorkResult> results)
        : base(results.Select(r => r.ToString()).Aggregate((a, b) => $"{a}\n{b}"))
        { 
            this.ScopeResults = results;
        }
    }
}
