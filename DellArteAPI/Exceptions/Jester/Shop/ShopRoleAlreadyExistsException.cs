namespace DellArteAPI.Exceptions.Shop;

public class ShopRoleAlreadyExistsException(
    long guildRoleId
) : Exception, ICustomException
{
    public string Code { get; set; } = "00589";
    public long GuildRoleId = guildRoleId;
}