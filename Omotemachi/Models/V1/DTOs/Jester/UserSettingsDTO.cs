using Omotemachi.Models.V1.Domain;
using Omotemachi.Models.V1.Domain.Jester.Settings;

namespace Omotemachi.Models.V1.DTOs.Jester;

public class SettingDTO
{
    public int Id { get; set; }
    public int Cost { get; set; }
    public bool Bought { get; set; }
    public bool State { get; set; }
    public SettingTypes Type { get; set; }
}

public class UserSettingsDTO
{
    public long GuildId { get; set; }
    public Guild Guild { get; set; }
    public long UserId { get; set; }
    public User User { get; set; }
    public List<SettingDTO> Settings { get; set; }
}