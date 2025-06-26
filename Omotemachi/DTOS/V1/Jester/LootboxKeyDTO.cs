using Omotemachi.Models.V1.Jester.Lootboxes;

namespace Omotemachi.DTOS.V1.Jester;

public class LootboxKeyDTO
{
    public LootboxTypes Type { get; set; }
    public int Quantity { get; set; }
}
