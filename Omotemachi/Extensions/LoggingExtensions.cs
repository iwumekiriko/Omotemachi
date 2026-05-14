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
        services.AddSingleton<LoggerProvider>();

        builder.Logging.Services.AddSingleton<ILoggerProvider>(sp =>
            sp.GetRequiredService<LoggerProvider>());

        return services;
    }
}