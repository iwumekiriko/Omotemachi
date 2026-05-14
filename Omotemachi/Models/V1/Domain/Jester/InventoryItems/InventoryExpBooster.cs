using Omotemachi.Models.V1.Domain.Jester;
using Omotemachi.Models.V1.Domain.Jester.Items;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Domain.Jester.InventoryItems;

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