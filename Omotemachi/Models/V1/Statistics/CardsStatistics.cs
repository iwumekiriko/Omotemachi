namespace Omotemachi.Models.V1.Statistics;

public class CardsStatistics : BaseStatistics
{
    public int PacksOpenedCount { get; set; }
    public int CardsSwappedToPackCount { get; set; }
    public int CardsTradedCount { get; set; }
    public int CardsGiftedCount { get; set; }

    public override object GetStats()
    {
        return new { PacksOpenedCount };
    }
}
