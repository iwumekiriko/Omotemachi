using Omotemachi.Models.V1.Jester.Lootboxes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester.Shop;

public class ShopKey
{
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild Guild { get; set; }
    public LootboxTypes LootboxType { get; set; }
    public bool Exclusive { get; set; } = false;
}
