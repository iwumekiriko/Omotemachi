using Omotemachi.Infrastructure.Background.Packs;
using Omotemachi.Infrastructure.Logging.Worker;

namespace Omotemachi.Extensions;

public static class BackgroundWorkerExtensions
{
    public static IServiceCollection AddBackgroundWorkers(this IServiceCollection services)
    {
        services.AddHostedService<LogWorker>();
        services.AddHostedService<PackMaintenanceWorker>();

        return services;
    }
}