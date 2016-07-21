using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESquare.Entity;
using LinqKit;
using System.Linq.Dynamic;
using ESquare.DAL;
using ESquare.Entity.Domain;

namespace ESquare.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseAggregateRoot
    {
        private readonly IDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(IDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public virtual TEntity Find(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual async Task<TEntity> FindAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _dbSet.FindAsync(cancellationToken, id);
        }

        public virtual IQueryable<TEntity> ExecuteSqlQuery(string query, params object[] parameters)
        {
            return _dbSet.SqlQuery(query, parameters).AsQueryable();
        }

        public virtual void Insert(TEntity entity)
        {
            if (entity != null)
            {
                entity.CreatedDate = DateTime.UtcNow;
                _dbSet.Add(entity);
            }
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            if (entities != null)
            {
                var currentTime = DateTime.UtcNow;
                var list = entities.ToList();
                list.ForEach(e => e.CreatedDate = currentTime);
                _dbSet.AddRange(list);
            }
        }

        public virtual void Update(TEntity entity)
        {
            if (entity != null)
            {
                entity.ModifiedDate = DateTime.UtcNow;
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            Delete(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
                return;

            _dbContext.Entry(entity).State = EntityState.Deleted;
            _dbSet.Attach(entity);
            _dbSet.Remove(entity);
        }

        public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entity = await FindAsync(id, cancellationToken);
            if (entity == null)
                return;

            _dbContext.Entry(entity).State = EntityState.Deleted;
            _dbSet.Attach(entity);
            _dbSet.Remove(entity);
        }

        public IFluentQuery<TEntity> Query()
        {
            return new FluentQuery<TEntity>(this);
        }

        public virtual IFluentQuery<TEntity> Query(IQueryObject<TEntity> queryObject)
        {
            return new FluentQuery<TEntity>(this, queryObject);
        }

        public virtual IFluentQuery<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return new FluentQuery<TEntity>(this, query);
        }

        internal IQueryable<TEntity> GetQueryable()
        {
            return _dbSet;
        }

        //internal IQueryable<TEntity> Select(
        //    Expression<Func<TEntity, bool>> filter = null,
        //    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        //    List<Expression<Func<TEntity, object>>> includes = null,
        //    int? page = null,
        //    int? pageSize = null)
        //{
        //    IQueryable<TEntity> query = _dbSet;

        //    if (includes != null)
        //    {
        //        query = includes.Aggregate(query, (current, include) => current.Include(include));
        //    }
        //    if (orderBy != null)
        //    {
        //        query = orderBy(query);
        //    }
        //    if (filter != null)
        //    {
        //        query = query.AsExpandable().Where(filter);
        //    }
        //    if (page != null && pageSize != null)
        //    {
        //        if (orderBy == null)
        //        {
        //            query = query.OrderBy(x => x.Id);
        //        }

        //        query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        //    }
        //    return query;
        //}

        //internal async Task<IEnumerable<TEntity>> SelectAsync(
        //    Expression<Func<TEntity, bool>> query = null,
        //    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        //    List<Expression<Func<TEntity, object>>> includes = null,
        //    int? page = null,
        //    int? pageSize = null)
        //{
        //    //See: Best Practices in Asynchronous Programming http://msdn.microsoft.com/en-us/magazine/jj991977.aspx
        //    return await Task.Run(() => Select(query, orderBy, includes, page, pageSize).AsEnumerable()).ConfigureAwait(false);
        //}
    }
}
