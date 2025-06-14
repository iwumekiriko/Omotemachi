using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.Models.V1.Jester.Config;

public class ExperienceConfig(long guildId) : IConfig
{
    [Key]
    public long GuildId { get; set; } = guildId;
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
    public int? ExpForMessage { get; set; } = 3;
    public int? ExpForVoiceMinute { get; set; } = 1;
    
}
