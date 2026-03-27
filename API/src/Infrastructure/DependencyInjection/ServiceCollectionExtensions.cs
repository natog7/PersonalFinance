using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using PersonalFinanceAPI.Application.Features.Finance.Services;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Application.Services;
using PersonalFinanceAPI.Domain.Services;
using PersonalFinanceAPI.Infrastructure.Persistence;
using PersonalFinanceAPI.Infrastructure.Repositories;
using PersonalFinanceAPI.Infrastructure.Security;
using PersonalFinanceAPI.Infrastructure.Services;
using StackExchange.Redis;

namespace PersonalFinanceAPI.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering Infrastructure layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Infrastructure layer services.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
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
        services.AddScoped<ICategoryRepository, CategoryRepository>();
		services.AddScoped<IUserRepository, UserRepository>();

		// Register Security Services
		services.AddScoped<ICurrentUserService, CurrentUserService>();
		services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<Application.Services.ITokenService, JwtTokenService>();

		return services;
    }

	public static IServiceCollection AddBalanceProjection(this IServiceCollection services, IConfiguration configuration)
	{
		// Redis
		services.AddSingleton<IConnectionMultiplexer>(
			ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
		services.AddScoped<IBalanceProjectionCacheService, BalanceProjectionCacheService>();

		// MongoDB
		services.AddSingleton<IMongoClient>(
			new MongoClient(configuration.GetConnectionString("MongoDB")));
		services.AddScoped<IMongoDatabase>(sp =>
			sp.GetRequiredService<IMongoClient>()
			  .GetDatabase(configuration["MongoDB:Database"]));
		services.AddScoped<IBalanceProjectionMongoRepository, BalanceProjectionMongoRepository>();

		// MassTransit + RabbitMQ
		services.AddMassTransit(x =>
		{
			x.AddConsumer<CalculateBalanceProjectionConsumer>();
			x.UsingRabbitMq((ctx, cfg) =>
			{
				cfg.Host(configuration.GetConnectionString("RabbitMQ"));
				cfg.ReceiveEndpoint("balance-projection-queue", e =>
				{
					e.ConfigureConsumer<CalculateBalanceProjectionConsumer>(ctx);
					e.UseMessageRetry(r => r.Intervals(500, 1000, 2000)); // retry com back-off
				});
			});
		});

		services.AddScoped<IBalanceProjectionProducer, BalanceProjectionProducer>();

		return services;
	}

	/// <summary>
	/// Applies pending migrations and creates the database if it doesn't exist.
	/// </summary>
	public static async Task<IServiceProvider> ApplyMigrationsAsync(this IServiceProvider serviceProvider)
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
