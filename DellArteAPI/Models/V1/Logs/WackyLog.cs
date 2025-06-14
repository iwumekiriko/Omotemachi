using DellArteAPI.Models.V1.Logs.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.Models.V1.Logs;

public class WackyLog : LogBase
{
    public WackyLogType Type { get; set; }
    public string? AvatarUrl { get; set; }

    public long? GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
    public long? UserId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }
}
