using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester.Config;

public class RolesConfig(long guildId) : IConfig
{
    [Key]
    public long GuildId { get; set; } = guildId;
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
    public long? SupportRoleId { get; set; }
    public long? ModeratorRoleId { get; set; }
    public long? DeveloperRoleId { get; set; }
}
