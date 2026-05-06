using Omotemachi.Models.V1.Domain.Logs;
using LogLevel = Omotemachi.Models.V1.Domain.Logs.LogLevel;
using System.Text.Json.Serialization;


namespace Omotemachi.Models.V1.DTOs.Logs;

public class LogDTO
{
    public long GuildId { get; set; } = 0;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LogLevel Level { get; set; }
    public string Message { get; set; }

    public string Category { get; set; }

    public long? UserId { get; set; }
    public string? AvatarUrl { get; set; }
}
