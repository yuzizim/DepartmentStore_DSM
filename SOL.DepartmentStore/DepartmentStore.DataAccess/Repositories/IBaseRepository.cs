// src/DepartmentStore.DataAccess/Repositories/IBaseRepository.cs
using System.Linq.Expressions;

namespace DepartmentStore.DataAccess.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdWithIncludeAsync(Guid id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindWithIncludeAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task SaveChangesAsync();
        Task<int> CountAsync();
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}