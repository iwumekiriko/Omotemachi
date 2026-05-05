namespace Omotemachi.Models.V1.Domain.Statistics;

public class DuetsStatistics : BaseStatistics
{
    public int DuetsCreatedCount { get; set; }
    public int DuetsDivorcedCount { get; set; }

    public override object GetStats()
    {
        return new { DuetsCreatedCount, DuetsDivorcedCount };
    }
}
