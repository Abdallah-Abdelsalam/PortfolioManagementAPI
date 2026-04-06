using PortfolioManagementAPI.Core.Entities;

namespace PortfolioManagementAPI.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserWithPortfoliosAsync(int userId);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> IsUniqueAsync(string username, string email);
}