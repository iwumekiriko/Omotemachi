using Microsoft.EntityFrameworkCore;
using Omotemachi.Infrastructure.Persistance.AppContext;
using Omotemachi.Models.V1.Domain.Jester.OW;
using Omotemachi.Tools;

namespace Omotemachi.Services.Jester;

public interface IOWService
{
    Task<UserHeroProgress> GetOrCreateAsync(long userId);
    Task UpdateHeroesAsync(long userId, string role, List<string> heroes);
    Task ResetAllHeroesAsync(long userId);
}

public class OWService(
    AppDbContext context,
    ILogger<DuetsService> logger
) : ServiceBase<DuetsService>(context, logger), IOWService
{
    public async Task UpdateHeroesAsync(long userId, string role, List<string> heroes)
    {
        var entity = await GetOrCreateAsync(userId);

        var dict = role.ToLower() switch
        {
            "tank" => entity.Progress.Tank,
            "damage" => entity.Progress.Damage,
            "support" => entity.Progress.Support,
            _ => throw new ArgumentException("Wrong role")
        };

        foreach (var hero in dict.Keys.ToList())
        {
            dict[hero] = heroes.Any(h =>
                string.Equals(h, hero, StringComparison.OrdinalIgnoreCase)) ? 1 : 0;
        }

        _context.Entry(entity).Property(x => x.Progress).IsModified = true;

        entity.UpdateAt = TimeConverter.GetCurrentTime();
        await _context.SaveChangesAsync();
    }

    public async Task ResetAllHeroesAsync(long userId)
    {
        var entity = await GetOrCreateAsync(userId);

        entity.Progress.Reset();
        _context.Entry(entity).Property(x => x.Progress).IsModified = true;

        entity.UpdateAt = TimeConverter.GetCurrentTime();
        await _context.SaveChangesAsync();
    }

    public async Task<UserHeroProgress> GetOrCreateAsync(long userId)
    {
        var entity = await _context.UserHeroProgresses
            .FirstOrDefaultAsync(uhp => uhp.UserId == userId);

        if (entity == null)
        {
            entity = new UserHeroProgress
            {
                UserId = userId,
                Progress = new HeroProgressData()
            };
            entity.Progress.EnsureAllHeroes();
            _context.UserHeroProgresses.Add(entity);
        }
        else
        {
            entity.Progress.EnsureAllHeroes();
        }

        await _context.SaveChangesAsync();
        return entity;
    }
}
