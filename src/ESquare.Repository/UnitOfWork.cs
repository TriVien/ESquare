using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContext _dbContext;

        private bool _disposed;
        private Hashtable _repositories;
        private DbContextTransaction _transaction;

        public UnitOfWork(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<T> ExecuteSqlQuery<T>(string sqlQuery, params object[] parameters)
        {
            return _dbContext.Database.SqlQuery<T>(sqlQuery, parameters);
        }

        public void ExecuteSqlCommand(string sqlCommand, params object[] parameters)
        {
            _dbContext.Database.ExecuteSqlCommand(sqlCommand, parameters);
        }

        //public DateTime GetDatabaseServerTime()
        //{
        //    var objectContext = ((IObjectContextAdapter)this).ObjectContext;
        //    var serverDateTime = objectContext.CreateQuery<DateTime>("CurrentDateTime() ").AsEnumerable().FirstOrDefault();
        //    return serverDateTime;
        //}

        public void SaveChanges(ConcurrencyResolutionMode mode = ConcurrencyResolutionMode.None)
        {
            bool saveFailed;

            switch (mode)
            {
                case ConcurrencyResolutionMode.None:
                    _dbContext.SaveChanges();
                    break;
                case ConcurrencyResolutionMode.DatabaseWin:
                    do
                    {
                        saveFailed = false;

                        try
                        {
                            _dbContext.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            saveFailed = true;

                            foreach (var failedEntry in ex.Entries)
                            {
                                failedEntry.Reload();
                            }
                        }

                    } while (saveFailed);

                    break;
                case ConcurrencyResolutionMode.ClientWin:
                    do
                    {
                        saveFailed = false;
                        try
                        {
                            _dbContext.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            saveFailed = true;

                            foreach (var failedEntry in ex.Entries)
                            {
                                failedEntry.OriginalValues.SetValues(failedEntry.GetDatabaseValues());
                            }
                        }

                    } while (saveFailed);


                    break;
                default:
                    _dbContext.SaveChanges();
                    break;
            }
        }

        public async Task SaveChangesAsync(ConcurrencyResolutionMode mode = ConcurrencyResolutionMode.None, CancellationToken cancellationToken = default(CancellationToken))
        {
            bool saveFailed;

            switch (mode)
            {
                case ConcurrencyResolutionMode.None:
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    break;
                case ConcurrencyResolutionMode.DatabaseWin:
                    do
                    {
                        saveFailed = false;

                        try
                        {
                            await _dbContext.SaveChangesAsync(cancellationToken);
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            saveFailed = true;

                            foreach (var failedEntry in ex.Entries)
                            {
                                failedEntry.Reload();
                            }
                        }

                    } while (saveFailed);

                    break;
                case ConcurrencyResolutionMode.ClientWin:
                    do
                    {
                        saveFailed = false;
                        try
                        {
                            await _dbContext.SaveChangesAsync(cancellationToken);
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            saveFailed = true;

                            foreach (var failedEntry in ex.Entries)
                            {
                                failedEntry.OriginalValues.SetValues(failedEntry.GetDatabaseValues());
                            }
                        }

                    } while (saveFailed);


                    break;
                default:
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    break;
            }
        }

        public T Reload<T>(T item) where T : BaseAggregateRoot
        {
            _dbContext.Entry(item).Reload();
            return item;
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _transaction = _dbContext.Database.BeginTransaction(isolationLevel);
        }

        public bool CommitTransaction()
        {
            _transaction.Commit();
            _transaction.Dispose();
            return true;
        }

        public void RollbackTransaction()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }

        public IRepository<T> Repository<T>() where T : BaseAggregateRoot
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                            .MakeGenericType(typeof(T)), _dbContext);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)_repositories[type];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
