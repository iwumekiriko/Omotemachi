namespace DellArteAPI.Models.V1.Jester.Statistics;

public class MessagesStatistics : BaseStatistics
{
    public int MessagesWritenCount { get; set; }

    public override object GetStats()
    {
        return new { MessagesWritenCount };
    }
}
