using PortfolioManagementAPI.Core.Entities;

namespace PortfolioManagementAPI.Core.Interfaces;

public interface IAuthService
{
    Task<string> GenerateJwtTokenAsync(User user);
    Task<string> GenerateRefreshTokenAsync();
    Task<User?> AuthenticateAsync(string username, string password);
    Task<bool> RegisterAsync(string username, string email, string password, string firstName, string lastName);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(User user);
}