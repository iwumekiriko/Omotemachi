using Omotemachi.Infrastructure.Logging.Queue;

namespace Omotemachi.Infrastructure.Logging.Logger;

public class LoggerProvider(ILogQueue queue) : ILoggerProvider
{
    private readonly ILogQueue _queue = queue;

    public ILogger CreateLogger(string categoryName)
    {
        return new Logger(categoryName, _queue);
    }
    public void Dispose() 
    { 
        GC.SuppressFinalize(this);
    }
}
