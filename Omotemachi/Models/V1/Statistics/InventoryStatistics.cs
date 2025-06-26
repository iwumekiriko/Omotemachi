namespace Omotemachi.Models.V1.Statistics;

public class InventoryStatistics : BaseStatistics
{
    public int ExpBoostersActivatedCount { get; set; }
    public int ExpGainedWithBoosters { get; set; }

    public override object GetStats()
    {
        return new { };
    }
}