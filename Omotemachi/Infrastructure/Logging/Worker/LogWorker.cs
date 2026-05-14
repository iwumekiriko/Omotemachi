using Microsoft.EntityFrameworkCore;
using Omotemachi.Infrastructure.Logging.Dispatcher;
using Omotemachi.Infrastructure.Logging.Queue;
using Omotemachi.Infrastructure.Persistance.AppContext;
using Omotemachi.Models.V1.Domain.Jester.Config;
using Omotemachi.Models.V1.Domain.Logs;
using Omotemachi.Tools;
using LogLevel = Omotemachi.Models.V1.Domain.Logs.LogLevel;

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
            if (IsConsoleLoggable(log))
            {
                WriteConsoleLog(log);
            }

            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (IsDbLoggable(log))
                await SaveLogAsync(context, log);

            if (log.Source == LogSource.External && log.GuildId != 0)
            {
                var dispatcher = scope.ServiceProvider.GetRequiredService<IWebhookDispatcher>();
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
    private static bool IsDbLoggable(LogEntry log)
    {
        return log.Source switch
        {
            LogSource.Framework => log.Level >= LogLevel.Warning,
            LogSource.Application => log.Level >= LogLevel.Information,
            LogSource.External => log.Level >= LogLevel.Information,
            _ => false
        };
    }
    private static bool IsConsoleLoggable(LogEntry log)
    {
        return log.Source switch
        {
            LogSource.Framework => log.Level >= LogLevel.Information,
            LogSource.Application => log.Level >= LogLevel.Debug,
            LogSource.External => log.Level >= LogLevel.Information,
            _ => false
        };
    }
    private static ConsoleColor GetLevelColor(LogLevel level)
    {
        return level switch
        {
            LogLevel.Debug => ConsoleColor.Cyan,
            LogLevel.Information => ConsoleColor.Green,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.DarkRed,
            LogLevel.Critical => ConsoleColor.Red,
            _ => ConsoleColor.White
        };
    }
    private static void WriteConsoleLog(LogEntry log)
    {
        var timestamp = TimeConverter.GetCurrentTime();
        var originalColor = Console.ForegroundColor;

        if (log.Source == LogSource.External)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[EXTERNAL] ");
            Console.ForegroundColor = originalColor;
            Console.Write("| ");
        }

        Console.Write($"[{timestamp}] ");

        Console.ForegroundColor = GetLevelColor(log.Level);
        Console.Write(log.Level.ToString().ToUpper());
        Console.ForegroundColor = originalColor;

        Console.WriteLine($": {log.Message.Replace("\n", "\\n")}");
    }
}