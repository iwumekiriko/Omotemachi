namespace Omotemachi.Models.V1.Statistics;

public class DNDStatistics : BaseStatistics
{
    public int DNDDiceRolledCount { get; set; }
    public int DNDDiceRolledMaxCount { get; set; }
    public int DNDDiceRolledMinCount { get; set; }

    public override object GetStats()
    {
        return new { DNDDiceRolledCount, DNDDiceRolledMinCount, DNDDiceRolledMaxCount };
    }
}