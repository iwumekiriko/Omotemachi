using Omotemachi.Models.V1.Domain.Jester.Lootboxes;

namespace Omotemachi.Models.V1.DTOs.Jester;

public class LootboxKeyDTO
{
    public LootboxTypes Type { get; set; }
    public int Quantity { get; set; }
}
