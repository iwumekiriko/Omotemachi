namespace DellArteAPI.Models.V1.Jester.Statistics;

public class TicketsStatistics : BaseStatistics
{
    public int SupportTicketCreatedCount {  get; set; }
    public int ModeratorTicketCreatedCount { get; set; }
    public int DeveloperTicketCreatedCount { get; set; }
    public int TicketsWasStartedCount { get; set; }
    public int TicketsWasClosedCount { get; set; }

    public override object GetStats()
    {
        return new { };
    }
}