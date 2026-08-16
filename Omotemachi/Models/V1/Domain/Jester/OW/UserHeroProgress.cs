using Omotemachi.Tools;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Domain.Jester.OW;

public class UserHeroProgress
{
    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    public HeroProgressData Progress { get; set; } = new();
    public DateTimeOffset UpdateAt { get; set; } = TimeConverter.GetCurrentTime();
}
