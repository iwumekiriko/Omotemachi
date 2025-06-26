namespace Omotemachi.Exceptions.Jester.Inventory;

public class AlreadyOwnsRoleException(long guildRoleId) : Exception, ICustomException
{
    public string Code { get; set; } = "01403";
    public long GuildRoleId { get; set; } = guildRoleId;
}
