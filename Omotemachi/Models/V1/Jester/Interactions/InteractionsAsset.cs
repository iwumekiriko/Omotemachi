namespace Omotemachi.Models.V1.Jester.Interactions;

public class InteractionsAsset
{
    public int Id { get; set; }
    public required string AssetUrl { get; set; }
    public InteractionsActions Action { get; set; }
    public InteractionsTypes Type { get; set; }
}