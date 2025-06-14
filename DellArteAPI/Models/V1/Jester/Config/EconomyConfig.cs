using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DellArteAPI.Models.V1.Jester.Config;

public class EconomyConfig(long guildId) : IConfig
{
    [Key]
    public long GuildId { get; set; } = guildId;
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
    public string DefaultCurrencyIcon { get; set; } = "🪙";
    public string DonateCurrencyIcon { get; set; } = "🥐";
    public int DailyBonus { get; set; } = 300;
}
