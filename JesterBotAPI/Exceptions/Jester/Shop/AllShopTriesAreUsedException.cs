using DellArteAPI.Models.V1.Jester.Lootboxes;

namespace DellArteAPI.Exceptions.Shop;

public class AllShopTriesAreUsedException(long guildRoleId) : Exception, ICustomException
{
    public string Code { get; set; } = "09921";
    public long GuildRoleId { get; set; } = guildRoleId;
}