using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester.Settings;

public class Setting
{
    public int Id { get; set; }
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
    public SettingTypes Type { get; set; }
    public int Cost { get; set; }
}