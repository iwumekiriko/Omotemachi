namespace Omotemachi.Models.V1.DTOs.Jester;

public class InventoryItemResponse<T> where T : class
{
    public int Quantity { get; set; }
    public T Item { get; set; }
}
