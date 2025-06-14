using DellArteAPI.DTOS.V1;
using DellArteAPI.Models.V1.Jester.Statistics;

namespace DellArteAPI.Services.Wacky;

public interface IDNDService
{
    Task ProcessRollResults(long guildId, long userId, List<RollResultDTO> rollResults);
}

public class DNDService(
    AppContext context,
    ILogger<DNDService> logger,
    IStatisticsService statistics
) : ServiceBase(context, logger), IDNDService
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