namespace DellArteAPI.Models.V1.Jester.Statistics;

public class QuestsStatistics : BaseStatistics
{
    public int QuestsAssignedCount { get; set; }
    public int QuestsCompletedCount { get; set; }

    public int CoinsFromQuestsCount { get; set; }
    public int CrystallsFromQuestsCount { get; set; }
    public int LootboxKeysFromQuestsCount { get; set; }

    public override object GetStats()
    {
        return new { QuestsCompletedCount };
    }
}
