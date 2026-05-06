using Microsoft.EntityFrameworkCore;
using Omotemachi.Infrastructure.Persistance.AppContext;

namespace Omotemachi.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
        
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            }));

        return services;
    }
}