using Microsoft.EntityFrameworkCore;
using PortfolioManagementAPI.Core.Entities;
using PortfolioManagementAPI.Core.Interfaces;
using PortfolioManagementAPI.Infrastructure.Data;

namespace PortfolioManagementAPI.Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Project>> GetProjectsByPortfolioIdAsync(int portfolioId)
    {
        return await _context.Projects
            .Where(p => p.PortfolioId == portfolioId && p.IsActive)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectWithDetailsAsync(int projectId)
    {
        return await _context.Projects
            .Include(p => p.Likes)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.IsActive);
    }

    public async Task<bool> ToggleLikeAsync(int projectId, int userId)
    {
        var existingLike = await _context.ProjectLikes
            .FirstOrDefaultAsync(pl => pl.ProjectId == projectId && pl.UserId == userId);

        if (existingLike != null)
        {
            _context.ProjectLikes.Remove(existingLike);
            await _context.SaveChangesAsync();
            return false;
        }

        var newLike = new ProjectLike
        {
            ProjectId = projectId,
            UserId = userId,
            LikedAt = DateTime.UtcNow
        };

        await _context.ProjectLikes.AddAsync(newLike);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetLikeCountAsync(int projectId)
    {
        return await _context.ProjectLikes
            .CountAsync(pl => pl.ProjectId == projectId);
    }

    public async Task<bool> IsLikedByUserAsync(int projectId, int userId)
    {
        return await _context.ProjectLikes
            .AnyAsync(pl => pl.ProjectId == projectId && pl.UserId == userId);
    }
}