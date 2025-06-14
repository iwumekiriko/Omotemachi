using DellArteAPI.Models.V1.Jester.Lootboxes;

namespace DellArteAPI.Exceptions.Shop;

public class ShopKeyAlreadyExistsException(
    LootboxTypes lootboxType
) : Exception, ICustomException
{
    public string Code { get; set; } = "00377";
    public LootboxTypes LootboxType = lootboxType;
}