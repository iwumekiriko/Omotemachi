using Omotemachi.Infrastructure.Logging.Logger;

namespace Omotemachi.Extensions;

public static class LoggingExtensions
{
    public static IServiceCollection AddLogging(
        this IServiceCollection services,
        WebApplicationBuilder builder
    )
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        services.AddSingleton<LoggerProvider>();

        builder.Logging.Services.AddSingleton<ILoggerProvider>(sp =>
            sp.GetRequiredService<LoggerProvider>());

        return services;
    }
}