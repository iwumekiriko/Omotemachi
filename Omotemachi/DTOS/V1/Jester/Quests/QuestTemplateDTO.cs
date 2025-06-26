using Omotemachi.Models.V1;
using Omotemachi.Models.V1.Jester.Quests;

namespace Omotemachi.DTOS.V1.Jester.Quests;

public class QuestTemplateDTO
{
    public long GuildId { get; set; }
    public QuestTypes Type { get; set; }
    public QuestTaskTypes TaskType { get; set; }
    public long? ChannelId { get; set; }
    public int Required { get; set; }
    public QuestRewardTypes RewardType { get; set; }
    public int RewardAmount { get; set; }
    public float Weight { get; set; }
}
