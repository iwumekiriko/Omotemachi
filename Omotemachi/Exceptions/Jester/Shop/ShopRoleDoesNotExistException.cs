namespace Omotemachi.Exceptions.Shop;

public class ShopRoleDoesNotExistException(
    long guildRoleId
) : Exception, ICustomException
{
    public string Code { get; set; } = "00590";
    public long GuildRoleId = guildRoleId;
}