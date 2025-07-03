using Omotemachi.Tools;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Wacky.Appa;

public class UserAppa
{
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild Guild { get; set; }

    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }

    public int AppaId { get; set; }
    [ForeignKey("AppaId")]
    public Appa Appa { get; set; }

    public int Amount { get; set; } = 0;
    public DateTimeOffset AcquiredAt { get; set; } = TimeConverter.GetCurrentTime();
}
