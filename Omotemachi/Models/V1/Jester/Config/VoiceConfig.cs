using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester.Config;

public class VoiceConfig(long guildId) : IConfig
{
    [Key]
    public long GuildId { get; set; } = guildId;
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
    public long? CustomVoiceCreationChannelId { get; set; }
    public long? CustomVoiceCategoryId { get; set; }
    public int? CustomVoiceDeletionTime { get; set; } = 30;
}
