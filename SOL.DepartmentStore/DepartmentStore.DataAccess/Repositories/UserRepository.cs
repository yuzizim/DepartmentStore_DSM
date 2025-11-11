using DepartmentStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DepartmentStore.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<List<AppUser>> FindAsync(Expression<Func<AppUser, bool>> predicate)
        {
            return await _context.Users.Where(predicate).ToListAsync();
        }
    }
}