using DellArteAPI.Models.V1.Jester.Settings;

namespace DellArteAPI.Exceptions.Jester.Settings;

public class GuildSettingDoesNotExistsException(long guildId, SettingTypes type) : Exception, ICustomException
{
    public string Code { get; set; } = "00302";
    public long GuildId { get; set; } = guildId;
    public SettingTypes Type { get; set; } = type;
}
