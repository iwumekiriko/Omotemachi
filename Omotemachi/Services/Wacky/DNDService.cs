using Omotemachi.Infrastructure.Persistance.AppContext;
using Omotemachi.Models.V1.Domain.Statistics;
using Omotemachi.Models.V1.DTOs.Wacky;

namespace Omotemachi.Services.Wacky;

public interface IDNDService
{
    Task ProcessRollResults(long guildId, long userId, List<RollResultDTO> rollResults);
}

public class DNDService(
    AppDbContext context,
    ILogger<DNDService> logger,
    IStatisticsService statistics
) : ServiceBase<DNDService>(context, logger), IDNDService
{
    private readonly IStatisticsService _statistics = statistics;

    public async Task ProcessRollResults(long guildId, long userId, List<RollResultDTO> rollResults)
    {
        foreach (var result in rollResults)
        {
            foreach (var roll in result.Rolls)
            {
                if (!(result.Parameters.Sides >= 20))
                    continue;

                if (roll == result.Parameters.Sides)
                    await _statistics.IncrementStatistics<DNDStatistics>(
                        guildId, userId, s => s.DNDDiceRolledMaxCount);

                else if (roll == 1)
                    await _statistics.IncrementStatistics<DNDStatistics>(
                        guildId, userId, s => s.DNDDiceRolledMinCount);
            }

            await _statistics.IncrementStatistics<DNDStatistics>(
                guildId, userId, s => s.DNDDiceRolledCount, result.Parameters.Throws);
        }
    }
}