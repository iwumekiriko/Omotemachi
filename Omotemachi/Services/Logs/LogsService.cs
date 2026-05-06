using Omotemachi.Infrastructure.Logging.Queue;
using Omotemachi.Infrastructure.Persistance.AppContext;
using Omotemachi.Models.V1.Domain.Logs;
using Omotemachi.Models.V1.DTOs.Logs;

namespace Omotemachi.Services.Logs;

public interface ILogsService
{
    Task HandleAsync(LogDTO dto);
}

public class LogsService(
    AppDbContext context,
    ILogger<LogsService> logger,
    ILogQueue queue
) : ServiceBase<LogsService>(context, logger), ILogsService
{
    private readonly ILogQueue _queue = queue;

    public Task HandleAsync(LogDTO dto)
    {
        var log = new LogEntry
        {
            Id = Guid.NewGuid(),

            Source = LogSource.External,
            GuildId = dto.GuildId,

            Level = dto.Level,
            Message = dto.Message,
            Category = dto.Category,

            AvatarUrl = dto.AvatarUrl,
            UserId = dto.UserId
        };

        _queue.Enqueue(log);
        return Task.CompletedTask;
    }
}
