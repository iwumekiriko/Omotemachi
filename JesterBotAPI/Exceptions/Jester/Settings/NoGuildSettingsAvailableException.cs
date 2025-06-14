namespace DellArteAPI.Exceptions.Jester.Settings;

public class NoGuildSettingsAvailableException(long guildId) : Exception, ICustomException
{
    public string Code { get; set; } = "00303";
    public long GuildId { get; set; } = guildId;
}
