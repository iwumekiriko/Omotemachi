using Omotemachi.Infrastructure.Logging.Logger;

namespace Omotemachi.Extensions;

public static class LoggingExtensions
{
    public static IServiceCollection AddCustomLogging(
        this IServiceCollection services,
        WebApplicationBuilder builder
    )
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
        builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);
        builder.Logging.AddFilter<LoggerProvider>(level =>
            level >= LogLevel.Information);

        services.AddSingleton<LoggerProvider>();

        builder.Logging.Services.AddSingleton<ILoggerProvider>(sp =>
            sp.GetRequiredService<LoggerProvider>());

        return services;
    }
}