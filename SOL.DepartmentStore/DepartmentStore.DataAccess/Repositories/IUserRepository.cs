using DepartmentStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DepartmentStore.DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<int> CountAsync();
        Task<List<AppUser>> FindAsync(Expression<Func<AppUser, bool>> predicate);
    }
}