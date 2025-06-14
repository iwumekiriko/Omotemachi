using DellArteAPI.Models.V1.Jester.Items;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.Models.V1.Jester.InventoryItems;

public class InventoryExpBooster : IInventoryItem
{
    [Key]
    public int Id { get; set; }
    public int InventoryId { get; set; }
    [ForeignKey("InventoryId")]
    public Inventory Inventory { get; set; }

    public int ExpBoosterId { get; set; }
    [ForeignKey("ExpBoosterId")]
    public ExpBooster ExpBooster { get; set; }

    public int Quantity { get; set; }
}