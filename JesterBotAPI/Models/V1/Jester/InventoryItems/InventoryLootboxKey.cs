using DellArteAPI.Models.V1.Jester.Items;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.Models.V1.Jester.InventoryItems;

public class InventoryLootboxKey : IInventoryItem
{
    [Key]
    public int Id { get; set; }
    public int InventoryId { get; set; }
    [ForeignKey("InventoryId")]
    public Inventory Inventory { get; set; }

    public int LootboxKeyId { get; set; }
    [ForeignKey("LootboxKeyId")]
    public LootboxKey LootboxKey { get; set; }

    public int Quantity { get; set; }
}