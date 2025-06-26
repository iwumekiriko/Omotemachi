using Omotemachi.Models.V1.Jester;
using Omotemachi.Models.V1.Statistics;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Omotemachi.Services.Jester;

public interface ITicketsService
{
    Task CreateTicket(Ticket ticket);
    Task<Ticket?> GetTicket(long id);
    Task StartTicket(Ticket updatedTicket);
    Task CloseTicket(Ticket updatedTicket);
}
public class TicketsService(
    AppContext context,
    ILogger<TicketsService> logger,
    IStatisticsService statistics
) : ServiceBase(context, logger), ITicketsService
{
    private readonly IStatisticsService _statistics = statistics;

    public async Task CreateTicket(Ticket ticket)
    {
        ticket.DateCreate = DateTime.Now;

        Expression<Func<TicketsStatistics, int>> propertySelector = ticket.TypeProblem switch
        {
            "moderator" => s => s.ModeratorTicketCreatedCount,
            "developer" => s => s.DeveloperTicketCreatedCount,
            _ => s => s.SupportTicketCreatedCount
        };

        await _statistics.IncrementStatistics(
            ticket.GuildId, ticket.UserId, propertySelector);

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
    }
    public async Task<Ticket?> GetTicket(long id)
    {
        return await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
    }
    public async Task StartTicket(Ticket updatedTicket)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == updatedTicket.Id) 
            ?? throw new ArgumentException(updatedTicket.Id.ToString());

        await _statistics.IncrementStatistics<TicketsStatistics>(
            ticket.GuildId, ticket.UserId, s => s.TicketsWasStartedCount);

        ticket.ModeratorId = updatedTicket.ModeratorId;
        await _context.SaveChangesAsync();
    }
    public async Task CloseTicket(Ticket updatedTicket)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == updatedTicket.Id)
            ?? throw new ArgumentException(updatedTicket.Id.ToString());

        await _statistics.IncrementStatistics<TicketsStatistics>(
            ticket.GuildId, ticket.UserId, s => s.TicketsWasClosedCount);

        ticket.Solution = updatedTicket.Solution;
        ticket.DateClose = DateTime.Now;
        await _context.SaveChangesAsync();
    }
}
