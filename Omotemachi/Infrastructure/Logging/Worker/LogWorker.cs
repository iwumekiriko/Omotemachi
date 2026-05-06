using Microsoft.EntityFrameworkCore;
using Omotemachi.Infrastructure.Dispatchers.Webhooks;
using Omotemachi.Infrastructure.Logging.Queue;
using Omotemachi.Infrastructure.Persistance.AppContext;
using Omotemachi.Models.V1.Domain.Jester.Config;
using Omotemachi.Models.V1.Domain.Logs;

namespace Omotemachi.Infrastructure.Logging.Worker;

public class LogWorker(
    IServiceProvider services,
    ILogQueue queue
) : BackgroundService
{
    private readonly IServiceProvider _services = services;
    private readonly ILogQueue _queue = queue;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var log in _queue.ReadAllAsync())
        {
            using var scope = _services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dispatcher = scope.ServiceProvider.GetRequiredService<IWebhookDispatcher>();

            await SaveLogAsync(context, log);

            if (!(log.GuildId == 0))
            {
                var config = await GetLogsConfigByGuild(context, log.GuildId);
                await dispatcher.DispatchAsync(log, config);
            }
        }
    }
    private static async Task SaveLogAsync(AppDbContext context, LogEntry log)
    {
        context.Logs.Add(log);
        await context.SaveChangesAsync();
    }
    private static async Task<LogsConfig> GetLogsConfigByGuild(AppDbContext context, long guildId)
    {
        var config = await context.LogsConfig
            .Where(lc => lc.GuildId == guildId)
            .FirstOrDefaultAsync();

        if (config == null)
        {
            config = new LogsConfig(guildId);
            context.LogsConfig.Add(config);
            await context.SaveChangesAsync();
        }

        return config;
    }
}