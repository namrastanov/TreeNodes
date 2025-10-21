using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TreeNodes.Application.Common.Interfaces;
using TreeNodes.Infrastructure.Database;

namespace TreeNodes.Infrastructure;

/// <summary>
/// Service registration extensions for Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services including DbContext.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "public");
            });
        });

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }
}


