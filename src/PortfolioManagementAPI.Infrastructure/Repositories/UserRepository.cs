using Microsoft.EntityFrameworkCore;
using PortfolioManagementAPI.Core.Entities;
using PortfolioManagementAPI.Core.Interfaces;
using PortfolioManagementAPI.Infrastructure.Data;

namespace PortfolioManagementAPI.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetUserWithPortfoliosAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Portfolios)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
    }

    public async Task<bool> IsUniqueAsync(string username, string email)
    {
        return !await _context.Users.AnyAsync(u => u.Username == username || u.Email == email);
    }
}