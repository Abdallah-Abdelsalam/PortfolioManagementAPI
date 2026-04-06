using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioManagementAPI.Core.Interfaces;
using PortfolioManagementAPI.Infrastructure.Data;
using PortfolioManagementAPI.Infrastructure.Repositories;
using PortfolioManagementAPI.Infrastructure.Services;

namespace PortfolioManagementAPI.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}