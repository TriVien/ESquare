using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESquare.Entity;
using ESquare.Entity.Domain;

namespace ESquare.DAL
{
    public interface IDbContext : IDisposable
    {
        Database Database { get; }

        DbSet<TEntity> Set<TEntity>() where TEntity : BaseAggregateRoot;
        DbEntityEntry Entry(object entity);
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
