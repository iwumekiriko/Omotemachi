using Omotemachi.Models.V1.Domain.Logs;
using System.Threading.Channels;

namespace Omotemachi.Infrastructure.Logging.Queue;

public interface ILogQueue
{
    void Enqueue(LogEntry log);
    IAsyncEnumerable<LogEntry> ReadAllAsync();
}

public class LogQueue : ILogQueue
{
    private readonly Channel<LogEntry> _channel = Channel.CreateUnbounded<LogEntry>();

    public void Enqueue(LogEntry log)
    {
        _channel.Writer.TryWrite(log);
    }

    public IAsyncEnumerable<LogEntry> ReadAllAsync()
    {
        return _channel.Reader.ReadAllAsync();
    }
}
