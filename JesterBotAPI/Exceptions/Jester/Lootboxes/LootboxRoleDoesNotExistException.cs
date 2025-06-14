using DellArteAPI.Models.V1.Jester.Lootboxes;

namespace DellArteAPI.Exceptions.Jester.Lootboxes;

public class LootboxRoleDoesNotExistException(
    LootboxTypes type, long guildRoleId
) : Exception, ICustomException
{
    public string Code { get; set; } = "00769";
    public LootboxTypes Type = type;
    public long GuildRoleId = guildRoleId;
}
