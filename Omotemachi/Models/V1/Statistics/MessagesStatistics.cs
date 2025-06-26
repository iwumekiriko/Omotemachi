namespace Omotemachi.Models.V1.Statistics;

public class MessagesStatistics : BaseStatistics
{
    public int MessagesWritenCount { get; set; }

    public override object GetStats()
    {
        return new { MessagesWritenCount };
    }
}
