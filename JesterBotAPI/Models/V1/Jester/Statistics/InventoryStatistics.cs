namespace DellArteAPI.Models.V1.Jester.Statistics;

public class InventoryStatistics : BaseStatistics
{
    public int ExpBoostersActivatedCount { get; set; }
    public int ExpGainedWithBoosters { get; set; }

    public override object GetStats()
    {
        return new { };
    }
}