using Hayat.DAL.Data;
using Hayat.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Hayat.DAL.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly HayatDbContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public GenericRepository(HayatDbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        protected IQueryable<TEntity> Query(bool asNoTracking = true)
        {
            var query = DbSet.AsQueryable();
            return asNoTracking ? query.AsNoTracking() : query;
        }

        public virtual async Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            return await DbSet.FindAsync(new[] { id }, cancellationToken);
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await Query().ToListAsync(cancellationToken);
        }

        public virtual async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Query().Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Query().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public virtual void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Query().AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate is null
                ? await Query().CountAsync(cancellationToken)
                : await Query().CountAsync(predicate, cancellationToken);
        }
    }
}
