using Omotemachi.Tools;
using System.ComponentModel.DataAnnotations;

namespace Omotemachi.Models.V1.Domain.Logs;

public class LogEntry
{
    [Key]
    public Guid Id { get; set; }
    public DateTimeOffset TimeStamp { get; set; } = TimeConverter.GetCurrentTime();

    public LogSource Source { get; set; }
    public long GuildId { get; set; }

    public LogLevel Level { get; set; }
    public required string Message { get; set; }
    public string? Category { get; set; }

    public string? AvatarUrl { get; set; }
    public string[] ImagesUrls { get; set; } = [];
    public string[] FilesUrls { get; set; } = [];
}