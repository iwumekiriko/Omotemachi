using Omotemachi.Models.V1.Jester.Settings;

namespace Omotemachi.Exceptions.Jester.Settings;

public class GuildSettingAlreadyExistsException(long guildId, SettingTypes type) : Exception, ICustomException
{
    public string Code { get; set; } = "00301";
    public long GuildId { get; set; } = guildId;
    public SettingTypes Type { get; set; } = type;
}
