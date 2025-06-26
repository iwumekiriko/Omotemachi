namespace Omotemachi.Services.Logs;

public interface ILogsService
{

}

public class LogsService(
    AppContext context,
    ILogger<LogsService> logger
) : ServiceBase(context, logger), ILogsService
{

}
