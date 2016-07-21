using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESquare.DAL;
using ESquare.Entity;
using ESquare.Entity.Domain;
using ESquare.Repository.Constants;

namespace ESquare.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Executes SQL query against database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlQuery">Query</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        IEnumerable<T> ExecuteSqlQuery<T>(string sqlQuery, params object[] parameters);

        /// <summary>
        /// Executes SQL command against database
        /// </summary>
        /// <param name="sqlCommand">Command</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        void ExecuteSqlCommand(string sqlCommand, params object[] parameters);

        /// <summary>
        /// Gets database server time
        /// </summary>
        /// <returns></returns>
        //DateTime GetDatabaseServerTime();

        /// <summary>
        /// Saves changes to database
        /// </summary>
        /// <param name="mode">Concurrency resolution mode</param>
        /// <returns></returns>
        void SaveChanges(ConcurrencyResolutionMode mode = ConcurrencyResolutionMode.None);

        /// <summary>
        /// Saves changes asynchronously
        /// </summary>
        /// <param name="mode">Concurrency resolution mode</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveChangesAsync(ConcurrencyResolutionMode mode = ConcurrencyResolutionMode.None, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Forces reloading data from database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">Item to reload</param>
        /// <returns></returns>
        T Reload<T>(T item) where T : BaseAggregateRoot;

        /// <summary>
        /// Begins a transaction
        /// </summary>
        /// <param name="isolationLevel">Isolation level</param>
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        /// <summary>
        /// Commits a transaction
        /// </summary>
        /// <returns></returns>
        bool CommitTransaction();

        /// <summary>
        /// Rollbacks a transaction
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// Gets a repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IRepository<T> Repository<T>() where T : BaseAggregateRoot;
    }
}
