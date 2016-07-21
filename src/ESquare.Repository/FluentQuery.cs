using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ESquare.Entity;
using LinqKit;
using System.Linq.Dynamic;
using ESquare.Entity.Domain;

namespace ESquare.Repository
{
    public class FluentQuery<TEntity> : IFluentQuery<TEntity> where TEntity : BaseAggregateRoot
    {
        #region Private Fields

        private IQueryable<TEntity> _query;
        private readonly Repository<TEntity> _repository;
        private bool _isSorted;

        #endregion Private Fields

        #region Constructors

        public FluentQuery(Repository<TEntity> repository)
        {
            _repository = repository;
            _query = repository.GetQueryable().AsExpandable();
        }

        public FluentQuery(Repository<TEntity> repository, IQueryObject<TEntity> queryObject)
            : this(repository)
        {
            _query = _query.Where(queryObject.Query());
        }

        public FluentQuery(Repository<TEntity> repository, Expression<Func<TEntity, bool>> filter)
            : this(repository)
        {
            _query = _query.Where(filter);
        }

        #endregion Constructors

        public IFluentQuery<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            _query = orderBy(_query);
            _isSorted = true;
            return this;
        }

        public IFluentQuery<TEntity> OrderBy(string orderBy)
        {
            _query = _query.OrderBy(orderBy);
            _isSorted = true;
            return this;
        }

        public IFluentQuery<TEntity> Include(Expression<Func<TEntity, object>> expression)
        {
            _query = _query.Include(expression);
            return this;
        }

        public IEnumerable<TEntity> SelectPaging(int page, int pageSize, out int totalCount)
        {
            if (!_isSorted)
            {
                _query = _query.OrderBy(x => x.Id);
            }

            totalCount = _query.Count();
            return _query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<IEnumerable<TEntity>> SelectPagingAsync(int page, int pageSize)
        {
            if (!_isSorted)
            {
                _query = _query.OrderBy(x => x.Id);
            }

            return await _query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public Task<int> CountAsync()
        {
            return _query.CountAsync();
        }

        public IEnumerable<TEntity> Select(int? maxItems = null)
        {
            if (maxItems.HasValue)
                return _query.Take(maxItems.Value);

            return _query.ToList();
        }

        public async Task<IEnumerable<TEntity>> SelectAsync(int? maxItems = null)
        {
            if (maxItems.HasValue)
                return _query.Take(maxItems.Value);

            return await _query.ToListAsync();
        }

        public IEnumerable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector, int? maxItems = null)
        {
            if (maxItems.HasValue)
                return _query.Take(maxItems.Value).Select(selector);

            return _query.Select(selector).ToList();
        }

        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, TResult>> selector, int? maxItems = null)
        {
            if (maxItems.HasValue)
                return _query.Take(maxItems.Value).Select(selector);

            return await _query.Select(selector).ToListAsync();
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string query, params object[] parameters)
        {
            return _repository.ExecuteSqlQuery(query, parameters).ToList();
        }

        public async Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string query, params object[] parameters)
        {
            return await _repository.ExecuteSqlQuery(query, parameters).ToListAsync();
        }
    }
}

