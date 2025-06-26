using Omotemachi.Models.V1.Jester.Lootboxes;

namespace Omotemachi.Exceptions.Jester.Lootboxes;

public class LootboxRoleAlreadyExistsException(
    LootboxTypes type, long guildRoleId    
) : Exception, ICustomException
{
    public string Code { get; set; } = "00767";
    public LootboxTypes Type = type;
    public long GuildRoleId = guildRoleId;
}
