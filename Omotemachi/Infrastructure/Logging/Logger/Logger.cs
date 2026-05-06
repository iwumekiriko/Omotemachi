using Omotemachi.Infrastructure.Logging.Queue;
using Omotemachi.Models.V1.Domain.Logs;

namespace Omotemachi.Infrastructure.Logging.Logger;

using MsLogLevel = Microsoft.Extensions.Logging.LogLevel;
using AppLogLevel = Models.V1.Domain.Logs.LogLevel;

public class Logger(string categoryName, ILogQueue queue) : ILogger
{
    private readonly ILogQueue _queue = queue;
    private readonly string _categoryName = categoryName;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull 
        => null;

    public bool IsEnabled(MsLogLevel logLevel)
        => logLevel != MsLogLevel.None;

    public void Log<TState>(
        MsLogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);

        var log = new LogEntry 
        {
            Id = Guid.NewGuid(),
            Message = message,
            Level = MapLevel(logLevel),
            Source = LogSource.API,
            Category = _categoryName,
            GuildId = 0
        };

        _queue.Enqueue(log);
    }

    private static AppLogLevel MapLevel(MsLogLevel level)
    {
        return level switch
        {
            MsLogLevel.Trace => AppLogLevel.Debug,
            MsLogLevel.Debug => AppLogLevel.Debug,
            MsLogLevel.Information => AppLogLevel.Info,
            MsLogLevel.Warning => AppLogLevel.Warning,
            MsLogLevel.Error => AppLogLevel.Error,
            MsLogLevel.Critical => AppLogLevel.Error,
            _ => AppLogLevel.Info
        };
    }
}
