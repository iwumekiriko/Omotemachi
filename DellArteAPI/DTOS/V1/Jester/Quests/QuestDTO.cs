using DellArteAPI.Models.V1;
using System.ComponentModel.DataAnnotations.Schema;
using DellArteAPI.Models.V1.Jester.Quests;

namespace DellArteAPI.DTOS.V1.Jester.Quests;

public class QuestDTO
{
    public int Id { get; set; }
    public long GuildId { get; set; }
    public Guild Guild { get; set; }
    public long? UserId { get; set; }
    public User? User { get; set; }
    public QuestTypes Type { get; set; }
    public QuestTaskTypes TaskType { get; set; }
    public long? ChannelId { get; set; }
    public int Required { get; set; }
    public int? Progress { get; set; }
    public QuestRewardTypes RewardType { get; set; }
    public int RewardAmount { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset CompletableUntil { get; set; }
    public bool IsCompleted { get; set; } = false;
    public bool? Valid { get; set; }
    public bool AcceptedByUser { get; set; } = false;
}
