namespace Omotemachi.Models.V1.Domain.Statistics;

public class MembersStatistics : BaseStatistics
{
    public int CoinsAmountChangedCount { get; set; }
    public int CrystalsAmountChangedCount { get; set; }

    public override object GetStats()
    {
        return new {  };
    }
}
