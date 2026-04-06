using Microsoft.EntityFrameworkCore;
using PortfolioManagementAPI.Core.Entities;
using PortfolioManagementAPI.Core.Interfaces;
using PortfolioManagementAPI.Infrastructure.Data;

namespace PortfolioManagementAPI.Infrastructure.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Comment>> GetCommentsByProjectIdAsync(int projectId)
    {
        return await _context.Comments
            .Include(c => c.User)
            .Where(c => c.ProjectId == projectId && c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId)
    {
        return await _context.Comments
            .Include(c => c.Project)
            .Where(c => c.UserId == userId && c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }
}