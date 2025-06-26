using Omotemachi.Models.V1.Statistics;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Omotemachi.Services;

public interface IStatisticsService
{
    Task<T> GetStatistics<T>(long guildId, long userId) where T : BaseStatistics, new();
    Task IncrementStatistics<T>(
        long guildId, long userId, Expression<Func<T, int>> propertySelector, int incrementBy = 1
    ) where T : BaseStatistics, new();
    Task SetStatistics<T>(
        long guildId, long userId, Expression<Func<T, int>> propertySelector, int value
    ) where T : BaseStatistics, new();
}

public class StatisticsService(
    AppContext context,
    ILogger<StatisticsService> logger
) : ServiceBase(context, logger), IStatisticsService
{
    public async Task<T> GetStatistics<T>(long guildId, long userId) where T : BaseStatistics, new()
    {
        var statistics = await _context.Set<T>()
            .FirstOrDefaultAsync(i => i.GuildId == guildId && i.UserId == userId);

        if (statistics == null)
        {
            statistics = new T() { GuildId = guildId, UserId = userId };
            _context.Set<T>().Add(statistics);
            await _context.SaveChangesAsync();
        }
        return statistics;
    }
    public async Task IncrementStatistics<T>(
        long guildId,
        long userId,
        Expression<Func<T, int>> propertySelector,
        int incrementBy = 1) where T : BaseStatistics, new()
    {
        bool exists = await _context.Set<T>()
            .AnyAsync(s => s.GuildId == guildId && s.UserId == userId);

        if (!exists)
        {
            var userStatistics = new T { GuildId = guildId, UserId = userId };
            _context.Set<T>().Add(userStatistics);
            await _context.SaveChangesAsync();
        }

        if (propertySelector.Body is MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;

            await _context.Set<T>()
                .Where(s => s.GuildId == guildId && s.UserId == userId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(
                        s => EF.Property<int>(s, propertyName),
                        s => EF.Property<int>(s, propertyName) + incrementBy
                    ));
        }
        else
        {
            throw new ArgumentException("incorrect sentence");
        }
    }
    public async Task SetStatistics<T>(
    long guildId,
    long userId,
    Expression<Func<T, int>> propertySelector,
    int value) where T : BaseStatistics, new()
    {
        bool exists = await _context.Set<T>()
            .AnyAsync(s => s.GuildId == guildId && s.UserId == userId);

        if (!exists)
        {
            var userStatistics = new T { GuildId = guildId, UserId = userId };
            _context.Set<T>().Add(userStatistics);
            await _context.SaveChangesAsync();
        }

        if (propertySelector.Body is MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;

            await _context.Set<T>()
                .Where(s => s.GuildId == guildId && s.UserId == userId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(
                        s => EF.Property<int>(s, propertyName),
                        value
                    ));
        }
        else
        {
            throw new ArgumentException("incorrect sentence");
        }
    }
}
