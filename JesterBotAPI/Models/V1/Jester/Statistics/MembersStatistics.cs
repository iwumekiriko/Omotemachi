namespace DellArteAPI.Models.V1.Jester.Statistics;

public class MembersStatistics : BaseStatistics
{
    public int CoinsAmountChangedCount { get; set; }
    public int CrystalsAmountChangedCount { get; set; }

    public override object GetStats()
    {
        return new {  };
    }
}
