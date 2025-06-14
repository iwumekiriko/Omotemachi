using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.Models.V1.Jester.Quests;

public class UserQuest
{
    public long UserId {  get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    public int QuestId { get; set; }
    [ForeignKey("QuestId")]
    public Quest Quest { get; set; }
    public int Progress { get; set; } = 0;
    public DateTime AssignedAt { get; set; } = DateTime.Now;
    public DateTimeOffset? CompletedAt { get; set; } = null;
    public bool IsCompleted { get; set; } = false;
}
