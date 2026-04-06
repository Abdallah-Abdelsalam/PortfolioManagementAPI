using PortfolioManagementAPI.Core.Entities;

namespace PortfolioManagementAPI.Core.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetCommentsByProjectIdAsync(int projectId);
    Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId);
}