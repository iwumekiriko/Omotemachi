using DellArteAPI.Exceptions.Jester.Duets;
using DellArteAPI.Models.V1;
using DellArteAPI.Models.V1.Jester;
using DellArteAPI.Models.V1.Jester.Settings;
using DellArteAPI.Models.V1.Jester.Statistics;
using Microsoft.EntityFrameworkCore;

namespace DellArteAPI.Services.Jester;

public interface IDuetsService
{
    Task<Duet?> GetDuet(long guildId, long userId);
    Task CreateDuet(long guildId, long proposerId, long duoId);
    Task DeleteDuet(long guildId, long userId);
}

public class DuetsService(
    AppContext context,
    ILogger<DuetsService> logger,
    IStatisticsService statistics,
    IUserSettingsService uSettingsService
) : ServiceBase(context, logger), IDuetsService
{
    private readonly IStatisticsService _statistics = statistics;
    private readonly IUserSettingsService _uSettingsService = uSettingsService;

    public async Task<Duet?> GetDuet(long guildId, long userId)
    {
        return await _context.Duets
            .FirstOrDefaultAsync(d => d.GuildId == guildId && d.Active
            && (d.ProposerId == userId || d.DuoId == userId));
    }
    public async Task CreateDuet(long guildId, long proposerId, long duoId)
    {
        var guild = await _context.Guilds
            .FirstOrDefaultAsync(g => g.Id == guildId)
            ?? new Guild { Id = guildId };
        var proposer = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == proposerId)
            ?? new User { Id = proposerId };
        var duo = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == duoId)
            ?? new User { Id = duoId };

        var duet = new Duet
        {
            Guild = guild,
            Proposer = proposer,
            Duo = duo,
            TogetherFrom = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
            Active = true
        };
        _context.Duets.Add(duet);

        await _statistics.IncrementStatistics<DuetsStatistics>(
            guildId, proposerId, s => s.DuetsCreatedCount);

        await _context.SaveChangesAsync();
    }
    public async Task DeleteDuet(long guildId, long userId)
    {
        if (await IsDisposeRestricted(guildId, userId))
            throw new DisposeRestrictedException();

        var duet = await _context.Duets
            .FirstAsync(d => d.GuildId == guildId
            && (d.ProposerId == userId || d.DuoId == userId));
        duet.Active = false;

        await _statistics.IncrementStatistics<DuetsStatistics>(
            guildId, userId, s => s.DuetsDivorcedCount);

        await _context.SaveChangesAsync();
    }
    private async Task<bool> IsDisposeRestricted(long guildId, long userId)
    {
        var duet = await GetDuet(guildId, userId);
        if (duet == null) return false;

        foreach (var targetId in new[] { duet.ProposerId, duet.DuoId })
        {
            var settings = await _uSettingsService.GetUserSetting(
                guildId,
                targetId,
                SettingTypes.RestrictDuetDispose
            );

            if (settings?.State == true)
                return true;
        }

        return false;
    }
}
