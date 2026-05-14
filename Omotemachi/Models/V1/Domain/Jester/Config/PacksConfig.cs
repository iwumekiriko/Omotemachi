using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Omotemachi.Models.V1.Domain;

namespace Omotemachi.Models.V1.Domain.Jester.Config;

public class PacksConfig(long guildId) : IConfig
{
    [Key]
    public long GuildId { get; set; } = guildId;
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
    public int PacksPrice { get; set; } = 2000;
}
