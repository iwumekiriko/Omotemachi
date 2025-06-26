using Omotemachi.Tools;

namespace Omotemachi.Models.V1.Logs;

public class LogBase
{
    public int Id { get; set; }
    public required LogLevel Level { get; set; } = LogLevel.Information;
    public required string Message { get; set; }
    public DateTimeOffset Timestamp { get; set; } = TimeConverter.GetCurrentTime();
}