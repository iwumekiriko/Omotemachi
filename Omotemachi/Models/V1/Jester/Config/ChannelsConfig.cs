using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester.Config;

public class ChannelsConfig(long guildId) : IConfig
{
    [Key]
    public long GuildId { get; set; } = guildId;
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
    public long? GeneralChannelId { get; set; }
    public long? OfftopChannelId { get; set; }
    public long? NitroBoostingChannelId { get; set; }
}
