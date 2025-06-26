using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester.InventoryItems;

public interface IInventoryItem
{
    public int Id { get; set; }
    public int InventoryId { get; set; }
    [ForeignKey("InventoryId")]
    public Inventory Inventory { get; set; }
}