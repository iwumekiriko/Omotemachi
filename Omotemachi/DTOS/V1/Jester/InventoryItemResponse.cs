namespace Omotemachi.DTOS.V1.Jester;

public class InventoryItemResponse<T> where T : class
{
    public int Quantity { get; set; }
    public T Item { get; set; }
}
