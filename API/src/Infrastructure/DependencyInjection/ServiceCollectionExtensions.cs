using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceAPI.Infrastructure.Persistence;
using PersonalFinanceAPI.Infrastructure.Repositories;
using PersonalFinanceAPI.Infrastructure.Security;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Application.Services;

namespace PersonalFinanceAPI.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering Infrastructure layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Infrastructure layer services.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly("PersonalFinanceAPI.Infrastructure");
                npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            });
        });

        // Register Repositories
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Register Security Services
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<Application.Services.ITokenService, JwtTokenService>();

        return services;
    }

    /// <summary>
    /// Applies pending migrations and creates the database if it doesn't exist.
    /// </summary>
    public static async Task<IServiceProvider> ApplyMigrationsAsync(
        this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            // Log the exception but don't throw - allows graceful handling
            Console.WriteLine($"Migration failed: {ex.Message}");
        }

        return serviceProvider;
    }
}
