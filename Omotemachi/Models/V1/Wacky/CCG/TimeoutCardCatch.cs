using Omotemachi.Tools;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Wacky.CCG;

public class TimeoutCardCatch
{
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild Guild { get; set; }
    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }

    public DateTimeOffset LastGive { get; set; } = TimeConverter.GetCurrentTime();
}
