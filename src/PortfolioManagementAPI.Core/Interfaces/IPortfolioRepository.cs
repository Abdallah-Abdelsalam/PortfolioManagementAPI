using PortfolioManagementAPI.Core.Entities;

namespace PortfolioManagementAPI.Core.Interfaces;

public interface IPortfolioRepository : IRepository<Portfolio>
{
    Task<IEnumerable<Portfolio>> GetPortfoliosByUserIdAsync(int userId);
    Task<Portfolio?> GetPortfolioWithProjectsAsync(int portfolioId);
}