using Omotemachi.Models.V1.Jester.Lootboxes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester.Config;

public class LootboxesConfig(long guildId) : IConfig
{
    [Key]
    public long GuildId { get; set; } = guildId;
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
    public int? GetKeyPrice(LootboxTypes type) => type switch
    {
        LootboxTypes.RolesLootbox => RolesLootboxKeyPrice,
        LootboxTypes.BackgroundsLootbox => BackgroundsLootboxKeyPrice,
        _ => null
    };

    public void SetKeyPrice(LootboxTypes type, int price)
    {
        switch (type)
        {
            case LootboxTypes.RolesLootbox:
                RolesLootboxKeyPrice = price;
                break;
            case LootboxTypes.BackgroundsLootbox:
                BackgroundsLootboxKeyPrice = price;
                break;
        }
    }

    public int? RolesLootboxKeyPrice { get; set; } = 3000;
    public int? BackgroundsLootboxKeyPrice { get; set; } = 3000;
    public string? ActiveLootboxes { get; set; }
}
