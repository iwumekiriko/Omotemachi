using Omotemachi.Models.V1.Jester.Quests;

namespace Omotemachi.Exceptions.Jester.Quests;

public class QuestTemplateDoesNotExistException(
    QuestTypes type, QuestTaskTypes task, int required, long? channelId
) : Exception, ICustomException
{
    public string Code { get; set; } = "01718";
    public QuestTypes Type { get; set; } = type;
    public QuestTaskTypes Task { get; set; } = task;
    public int Required { get; set; } = required;
    public long? ChannelId { get; set; } = channelId;
}