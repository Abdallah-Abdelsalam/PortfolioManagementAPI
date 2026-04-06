using Microsoft.EntityFrameworkCore;
using PortfolioManagementAPI.Core.Entities;
using PortfolioManagementAPI.Core.Interfaces;
using PortfolioManagementAPI.Infrastructure.Data;

namespace PortfolioManagementAPI.Infrastructure.Repositories;

public class PortfolioRepository : Repository<Portfolio>, IPortfolioRepository
{
    public PortfolioRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Portfolio>> GetPortfoliosByUserIdAsync(int userId)
    {
        return await _context.Portfolios
            .Where(p => p.UserId == userId && p.IsActive)
            .Include(p => p.Projects)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Portfolio?> GetPortfolioWithProjectsAsync(int portfolioId)
    {
        return await _context.Portfolios
            .Include(p => p.Projects)
            .ThenInclude(p => p.Likes)
            .Include(p => p.Projects)
            .ThenInclude(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == portfolioId && p.IsActive);
    }
}