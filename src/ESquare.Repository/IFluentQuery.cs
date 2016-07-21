using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ESquare.Entity;
using ESquare.Entity.Domain;

namespace ESquare.Repository
{
    public interface IFluentQuery<TEntity> where TEntity : BaseAggregateRoot
    {
        /// <summary>
        /// Orders by
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IFluentQuery<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);

        /// <summary>
        /// Orders by
        /// </summary>
        /// <param name="orderBy">Sort definition in the format "Field1 asc,Field2 desc,Field3 asc"</param>
        /// <returns></returns>
        IFluentQuery<TEntity> OrderBy(string orderBy);

        /// <summary>
        /// Includes any properties for eager loading
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IFluentQuery<TEntity> Include(Expression<Func<TEntity, object>> expression);

        /// <summary>
        /// Gets data
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <param name="maxItems"></param>
        /// <returns></returns>
        IEnumerable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector, int? maxItems = null);

        /// <summary>
        /// Gets data
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <param name="maxItems"></param>
        /// <returns></returns>
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, TResult>> selector, int? maxItems = null);

        /// <summary>
        /// Gets data
        /// </summary>
        /// <param name="maxItems"></param>
        /// <returns></returns>
        IEnumerable<TEntity> Select(int? maxItems = null);

        /// <summary>
        /// Gets data
        /// </summary>
        /// <param name="maxItems"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> SelectAsync(int? maxItems = null);

        /// <summary>
        /// Gets data with paging
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IEnumerable<TEntity> SelectPaging(int page, int pageSize, out int totalCount);

        /// <summary>
        /// Gets data with paging
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> SelectPagingAsync(int page, int pageSize);

        /// <summary>
        /// Counts data
        /// </summary>
        /// <returns></returns>
        Task<int> CountAsync();

        /// <summary>
        /// Executes SQL query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IEnumerable<TEntity> ExecuteSqlQuery(string query, params object[] parameters);

        /// <summary>
        /// Executes SQL query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string query, params object[] parameters);
    }
}
