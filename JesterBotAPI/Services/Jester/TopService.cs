using DellArteAPI.DTOS.V1.Jester;
using DellArteAPI.Models.V1.Jester.Statistics;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DellArteAPI.Services.Jester;

public interface ITopService
{
    Task<TopDTO> GetGenericTopFromStatisticsAsync<T>(
        IQueryable<T> source,
        Expression<Func<T, long>> orderSelector,
        long guildId, long userId) where T : BaseStatistics;

    Task<TopDTO> GetExperienceTopAsync(long guildId, long userId);
    Task<TopDTO> GetCurrencyTopAsync(long guildId, long userId);
    Task<TopDTO> GetDuetsTopAsync(long guildId, long userId);
}
public class TopService(
    AppContext context,
    ILogger<TopService> logger
) : ServiceBase(context, logger), ITopService
{
    private const int TOP_AMOUNT = 10;
    
    public async Task<TopDTO> GetGenericTopFromStatisticsAsync<T>(
        IQueryable<T> source,
        Expression<Func<T, long>> orderSelector,
        long guildId, long userId) where T : BaseStatistics
    {
        var statistics = await source
            .Where(s => s.GuildId == guildId)
            .OrderByDescending(orderSelector)
            .Take(TOP_AMOUNT)
            .ToListAsync();

        var userInTop = statistics
            .FirstOrDefault(s => s.UserId == userId);

        var userStats = userInTop != null
            ? statistics.First(s => s.UserId == userId)
            : await source.FirstOrDefaultAsync(
                s => s.UserId == userId && s.GuildId == guildId);

        var requestedUser = await CalculateUserRank(
            source, orderSelector, guildId, userId,
            userStats, userInTop, statistics);

        return new TopDTO
        {
            Top = [.. statistics
                .Select((s, index) => new UserRank
                {
                    Rank = index + 1,
                    GuildId = s.GuildId,
                    UserId = s.UserId,
                    Stats = s.GetStats()
                })],
            RequestedUser = requestedUser
        };
    }
    private static async Task<UserRank> CalculateUserRank<T>(
        IQueryable<T> source,
        Expression<Func<T, long>> orderSelector,
        long guildId, long userId,
        T? userStats,
        T? userInTop,
        List<T> statistics) where T : BaseStatistics
    {
        if (userInTop != null)
        {
            return new UserRank
            {
                Rank = statistics.IndexOf(userInTop) + 1,
                UserId = userId,
                GuildId = guildId,
                Stats = userStats!.GetStats()
            };
        }

        if (userStats == null)
        {
            var totalCount = await source
                .CountAsync(s => s.GuildId == guildId);
            return new UserRank
            {
                Rank = totalCount + 1,
                UserId = userId,
                GuildId = guildId,
                Stats = new { }
            };
        }

        var userValue = orderSelector.Compile()(userStats);
        var lambda = Expression.Lambda<Func<T, bool>>(
            Expression.GreaterThan(orderSelector.Body, Expression.Constant(userValue)),
            orderSelector.Parameters[0]
        );

        var usersAboveCount = await source
            .Where(s => s.GuildId == guildId)
            .CountAsync(lambda);

        return new UserRank
        {
            Rank = usersAboveCount + 1,
            GuildId = guildId,
            UserId = userId,
            Stats = userStats.GetStats()
        };
    }
    public async Task<TopDTO> GetExperienceTopAsync(long guildId, long userId)
    {
        var topMembers = await _context.Members
            .Where(m => m.GuildId == guildId && !m.IsBot)
            .OrderByDescending(ms => ms.Experience)
            .Take(TOP_AMOUNT)
            .ToListAsync();

        var userInTop = topMembers.FirstOrDefault(ms => ms.UserId == userId);
        var memberData = userInTop != null
            ? topMembers.First(m => m.UserId == userId)
            : _context.Members.FirstOrDefault(
                m => m.UserId == userId && m.GuildId == guildId);

        UserRank? requestedUser;

        if (userInTop != null)
        {
            requestedUser = new UserRank
            {
                Rank = topMembers.IndexOf(userInTop) + 1,
                GuildId = guildId,
                UserId = userId,
                Stats = new { memberData!.Experience }
            };
        }

        else if (memberData == null)
        {
            var totalCount = await _context.Members
                .CountAsync(s => s.GuildId == guildId);

            requestedUser = new UserRank
            {
                Rank = totalCount + 1,
                UserId = userId,
                GuildId = guildId,
                Stats = new { }
            };
        }

        else
        {
            var usersAboveCount = await _context.Members
                .Where(s => s.GuildId == guildId)
                .CountAsync(m => m.Experience > memberData!.Experience);

            requestedUser = new UserRank
            {
                Rank = usersAboveCount + 1,
                UserId = userId,
                GuildId = guildId,
                Stats = new { memberData.Experience }
            };
        }

        return new TopDTO
        {
            Top = [.. topMembers
                .Select((m, index) => new UserRank
                {
                    Rank = index + 1,
                    GuildId = m.GuildId,
                    UserId = m.UserId,
                    Stats = new { m.Experience }
                })],
            RequestedUser = requestedUser
        };
    }
    public async Task<TopDTO> GetCurrencyTopAsync(long guildId, long userId)
    {
        var topMembers = await _context.Members
            .Where(m => m.GuildId == guildId && !m.IsBot)
            .OrderByDescending(ms => ms.Coins + ms.Crystals * 100)
            .Take(TOP_AMOUNT)
            .ToListAsync();

        var userInTop = topMembers.FirstOrDefault(ms => ms.UserId == userId);
        var memberData = userInTop != null
            ? topMembers.First(m => m.UserId == userId)
            : _context.Members.FirstOrDefault(
                m => m.UserId == userId && m.GuildId == guildId);

        UserRank? requestedUser;

        if (userInTop != null)
        {
            requestedUser = new UserRank
            {
                Rank = topMembers.IndexOf(userInTop) + 1,
                GuildId = guildId,
                UserId = userId,
                Stats = new { memberData!.Coins, memberData!.Crystals }
            };
        }

        else if (memberData == null)
        {
            var totalCount = await _context.Members
                .CountAsync(s => s.GuildId == guildId);

            requestedUser = new UserRank
            {
                Rank = totalCount + 1,
                UserId = userId,
                GuildId = guildId,
                Stats = new { }
            };
        }

        else
        {
            var usersAboveCount = await _context.Members
                .Where(s => s.GuildId == guildId)
                .CountAsync(m => 
                    m.Coins + m.Crystals * 100 >
                    memberData!.Coins + memberData!.Crystals);

            requestedUser = new UserRank
            {
                Rank = usersAboveCount + 1,
                UserId = userId,
                GuildId = guildId,
                Stats = new { memberData.Coins, memberData.Crystals }
            };
        }

        return new TopDTO
        {
            Top = [.. topMembers
                .Select((m, index) => new UserRank
                {
                    Rank = index + 1,
                    GuildId = m.GuildId,
                    UserId = m.UserId,
                    Stats = new { m.Coins, m.Crystals }
                })],
            RequestedUser = requestedUser
        };
    }
    public async Task<TopDTO> GetDuetsTopAsync(long guildId, long userId)
    {
        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var topDuets = await _context.Duets
            .Where(d => d.GuildId == guildId)
            .OrderByDescending(d =>
                currentTime - d.TogetherFrom)
            .Take(TOP_AMOUNT)
            .ToListAsync();

        var userInTop = topDuets.FirstOrDefault(d =>
            d.ProposerId == userId || d.DuoId == userId);
        var duetData = userInTop != null
            ? topDuets.First(d => d.ProposerId == userId || d.DuoId == userId)
            : _context.Duets.FirstOrDefault(
                d => (d.ProposerId == userId || d.DuoId == userId) && d.GuildId == guildId);

        UserRank? requestedUser;

        if (userInTop != null)
        {
            requestedUser = new UserRank
            {
                Rank = topDuets.IndexOf(userInTop) + 1,
                GuildId = guildId,
                UserId = userId,
                Stats = new {
                    duetData!.ProposerId,
                    duetData!.DuoId,
                    duetData!.TogetherFrom
                }
            };
        }

        else if (duetData == null)
        {
            var totalCount = await _context.Duets
                .CountAsync(s => s.GuildId == guildId);

            requestedUser = new UserRank
            {
                Rank = totalCount + 1,
                UserId = userId,
                GuildId = guildId,
                Stats = new { }
            };
        }

        else
        {
            var usersAboveCount = await _context.Duets
                .Where(s => s.GuildId == guildId)
                .CountAsync(d =>
                    currentTime - d.TogetherFrom >
                    currentTime - duetData.TogetherFrom);

            requestedUser = new UserRank
            {
                Rank = usersAboveCount + 1,
                UserId = userId,
                GuildId = guildId,
                Stats = new {
                    duetData!.ProposerId,
                    duetData!.DuoId,
                    duetData!.TogetherFrom
                }
            };
        }

        return new TopDTO
        {
            Top = [.. topDuets
                .Select((d, index) => new UserRank
                {
                    Rank = index + 1,
                    GuildId = d.GuildId,
                    UserId = d.ProposerId,
                    Stats = new {
                        d.ProposerId,
                        d.DuoId,
                        d.TogetherFrom
                    }
                })],
            RequestedUser = requestedUser
        };
    }
}
