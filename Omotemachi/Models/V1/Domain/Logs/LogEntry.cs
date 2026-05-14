using Omotemachi.Tools;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Domain.Logs;

public class LogEntry
{
    [Key]
    public Guid Id { get; set; }
    public DateTimeOffset TimeStamp { get; set; } = TimeConverter.GetCurrentTime();

    public LogSource Source { get; set; }
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }

    public LogLevel Level { get; set; }
    public required string Message { get; set; }
    public string? Category { get; set; }

    public string? AvatarUrl { get; set; }
    public string[] ImagesUrls { get; set; } = [];
    public string[] FilesUrls { get; set; } = [];
}