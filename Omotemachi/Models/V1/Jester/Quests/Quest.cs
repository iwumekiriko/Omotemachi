using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester.Quests;

public class Quest
{
    public int Id { get; set; }
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild Guild { get; set; }
    public QuestTypes Type { get; set; }
    public QuestTaskTypes TaskType { get; set; }
    public long? ChannelId { get; set; }
    public int Required { get; set; }
    public QuestRewardTypes RewardType { get; set; }
    public int RewardAmount { get; set; }
    public float Weight { get; set; }
    public bool IsAvailableNow { get; set; }
    public DateTimeOffset CompletableUntil { get; set; } = DateTime.Now;
    public bool IsValid { get; set; } = true;
}