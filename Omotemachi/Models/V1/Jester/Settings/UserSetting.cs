using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester.Settings;

public class UserSetting
{
    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }
    public int SettingId { get; set; }
    [ForeignKey("SettingId")]
    public Setting Setting { get; set; }
    public bool State { get; set; }
}
