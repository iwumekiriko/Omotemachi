using DellArteAPI.Models.V1.Jester.Lootboxes;

namespace DellArteAPI.Exceptions.Shop;

public class ShopKeyDoesNotExistException(
    LootboxTypes lootboxType
) : Exception, ICustomException
{
    public string Code { get; set; } = "00378";
    public LootboxTypes LootboxType = lootboxType;
}