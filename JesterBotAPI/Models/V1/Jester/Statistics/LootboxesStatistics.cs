using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.Models.V1.Jester.Statistics;

public class LootboxesStatistics : BaseStatistics
{
    public int RolesLootboxesOpenedCount { get; set; }
    public int BackgroundsLootboxesOpenedCount { get; set; }

    [NotMapped]
    public int AllLootboxesOpenedCount => 
        RolesLootboxesOpenedCount + BackgroundsLootboxesOpenedCount;

    public int LootboxesRolesDroppedCount { get; set; }
    public int LootboxesBackgroundsDroppedCount { get; set; }

    public override object GetStats()
    {
        return new { AllLootboxesOpenedCount };
    }
}