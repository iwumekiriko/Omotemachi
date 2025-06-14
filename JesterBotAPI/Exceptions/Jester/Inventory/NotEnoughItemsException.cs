using DellArteAPI.Models.V1.Jester.Items;

namespace DellArteAPI.Exceptions.Jester.Inventory;

public class NotEnoughItemsException(
    int current, int needed, string itemType
) : Exception, ICustomException
{
    public string ItemType { get; } = itemType;
    public int Current { get; } = current;
    public int Needed { get; } = needed;
    public string Code { get; } = "00242";
}