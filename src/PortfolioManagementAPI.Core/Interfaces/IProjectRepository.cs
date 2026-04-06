using PortfolioManagementAPI.Core.Entities;

namespace PortfolioManagementAPI.Core.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetProjectsByPortfolioIdAsync(int portfolioId);
    Task<Project?> GetProjectWithDetailsAsync(int projectId);
    Task<bool> ToggleLikeAsync(int projectId, int userId);
    Task<int> GetLikeCountAsync(int projectId);
    Task<bool> IsLikedByUserAsync(int projectId, int userId);
}