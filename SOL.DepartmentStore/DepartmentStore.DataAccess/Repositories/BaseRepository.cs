// src/DepartmentStore.DataAccess/Repositories/BaseRepository.cs
using DepartmentStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DepartmentStore.DataAccess.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        // === 1. GET BY ID (NO INCLUDE) ===
        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        // === 2. GET ALL WITH OPTIONAL INCLUDES ===
        public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Áp dụng Soft Delete nếu T là BaseEntity
            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var condition = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);
                query = query.Where(lambda);
            }

            // Áp dụng Includes
            query = includes.Aggregate(query, (q, inc) => q.Include(inc));

            return await query.ToListAsync();
        }

        // === 3. GET ALL WITH INCLUDE + SOFT DELETE ===
        public virtual async Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
            }

            query = includes.Aggregate(query, (q, inc) => q.Include(inc));
            return await query.ToListAsync();
        }

        // === 4. GET BY ID WITH INCLUDE + SOFT DELETE ===
        public virtual async Task<T?> GetByIdWithIncludeAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((BaseEntity)(object)e).IsDeleted && ((BaseEntity)(object)e).Id == id);
            }
            else
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var idProperty = typeof(T).GetProperty("Id")?.GetGetMethod();
                if (idProperty != null)
                {
                    var idExpression = Expression.Call(parameter, idProperty);
                    var condition = Expression.Equal(idExpression, Expression.Constant(id));
                    var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);
                    query = query.Where(lambda);
                }
            }

            query = includes.Aggregate(query, (q, inc) => q.Include(inc));
            return await query.FirstOrDefaultAsync();
        }

        // === 5. FIND WITH INCLUDE + SOFT DELETE ===
        public virtual async Task<IEnumerable<T>> FindWithIncludeAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
            }

            query = includes.Aggregate(query, (q, inc) => q.Include(inc));
            return await query.Where(predicate).ToListAsync();
        }

        // === 6. CRUD ===
        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Remove(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsDeleted = true;
                baseEntity.UpdatedAt = DateTime.UtcNow;
                _dbSet.Update(entity);
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // === 7. UTILITIES ===
        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}