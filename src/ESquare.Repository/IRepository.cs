using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESquare.Entity;
using ESquare.Entity.Domain;

namespace ESquare.Repository
{
    public interface IRepository<TEntity> where TEntity : BaseAggregateRoot
    {
        /// <summary>
        /// Finds by primary keys
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity Find(int id);

        /// <summary>
        /// Finds by primary keys asynchronously
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TEntity> FindAsync(int id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Executes SQL query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IQueryable<TEntity> ExecuteSqlQuery(string query, params object[] parameters);

        /// <summary>
        /// Inserts Entity
        /// </summary>
        /// <param name="entity"></param>
        void Insert(TEntity entity);

        /// <summary>
        /// Inserts mulitple entities
        /// </summary>
        /// <param name="entities"></param>
        void InsertRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);

        /// <summary>
        /// Deletes entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        void Delete(int id);

        /// <summary>
        /// Deletes entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes entity asynchronously
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAsync(int id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Does querying
        /// </summary>
        /// <param name="queryObject"></param>
        /// <returns></returns>
        IFluentQuery<TEntity> Query(IQueryObject<TEntity> queryObject);

        /// <summary>
        /// Does querying
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IFluentQuery<TEntity> Query(Expression<Func<TEntity, bool>> query);

        /// <summary>
        /// Does querying
        /// </summary>
        /// <returns></returns>
        IFluentQuery<TEntity> Query();
    }
}
