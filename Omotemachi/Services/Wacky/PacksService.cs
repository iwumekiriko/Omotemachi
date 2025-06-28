using Omotemachi.Exceptions.Wacky.CCG;
using Omotemachi.Models.V1.Wacky.CCG;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Omotemachi.DTOS.V1.Wacky;

namespace Omotemachi.Services.Wacky;

public interface IPacksService
{
    Task<int?> GetGeneralPackId();
    Task<Pack?> GetPackAsync(int packId);
    Task<List<int>> GetRandomPackCardsIds(int packId, int amount);
    Task<List<PackDTO>> GetAllAvailablePacks(long guildId, long userId);
    Task<int> UpdateUserPackAmount(long guildId, long userId, int packId, int amount);
}
public class PacksService(
    AppContext context,
    ILogger<PacksService> logger
) : ServiceBase(context, logger), IPacksService
{
    private readonly Random _random = new();
    private static readonly ConcurrentDictionary<int, Pack> _packsCache = new();
    private static DateTime _lastCacheUpdate;

    public async Task<int?> GetGeneralPackId()
    {
        return (await _context.Packs
            .FirstOrDefaultAsync(p => p.Name == "General"))?.Id;
    }

    public async Task<Pack?> GetPackAsync(int packId)
    {
        if (_packsCache.TryGetValue(packId, out var cachedPack) &&
            (DateTime.Now - _lastCacheUpdate).TotalHours < 1)
        {
            return cachedPack;
        }

        var pack = await _context.Packs
            .Include(p => p.Cards)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == packId);

        if (pack != null)
        {
            _packsCache.AddOrUpdate(packId, pack, (id, old) => pack);
            _lastCacheUpdate = DateTime.Now;
        }

        return pack;
    }

    public async Task<List<int>> GetRandomPackCardsIds(int packId, int amount)
    {
        var pack = await GetPackAsync(packId);
        if (pack == null)
            return [];

        var cardIds = pack.Cards.Select(c => c.Id).ToList();
        var drops = new List<int>(amount);

        for (int i = 0; i < amount; i++)
        {
            int index = _random.Next(0, cardIds.Count);
            drops.Add(cardIds[index]);
        }

        return drops;
    }

    public async Task<List<PackDTO>> GetAllAvailablePacks(long guildId, long userId)
    {
        var packs = await _context.Packs
            .Where(p => p.Active)
            .ToListAsync();

        var userPacks = await _context.UserPacks
            .Where(up => up.GuildId == guildId && up.UserId == userId)
            .ToListAsync();

        var upDict = userPacks
            .GroupBy(up => up.PackId)
            .ToDictionary(g => g.Key, g => g.First().Amount);

        return [.. packs.Select(p => new PackDTO
        {
            Id = p.Id,
            Name = p.Name,
            Amount = upDict.TryGetValue(p.Id, out var amount) ? amount : 0
        })];
    }

    public async Task<int> UpdateUserPackAmount(long guildId, long userId, int packId, int amount)
    {
        var uPack = await _context.UserPacks
            .FirstOrDefaultAsync(up =>
                up.GuildId == guildId &&
                up.UserId == userId &&
                up.PackId == packId);

        if (uPack == null)
        {
            if (!await _context.Packs.AnyAsync(p => p.Id == packId))
                throw new ArgumentException($"No pack with id {packId}");

            if (amount < 0)
            {
                var packName = await _context.Packs
                    .Where(p => p.Id == packId)
                    .Select(p => p.Name)
                    .FirstAsync();

                throw new NotEnoughPacksException(
                    packName: packName,
                    packsAmount: 0,
                    packsNeeded: Math.Abs(amount)
                );
            }

            uPack = new UserPack
            {
                GuildId = guildId,
                UserId = userId,
                PackId = packId,
            };
            _context.UserPacks.Add(uPack);
        }

        uPack.Amount += amount;
        if (uPack.Amount < 0)
        {
            var packName = await _context.Packs
                .Where(p => p.Id == packId)
                .Select(p => p.Name)
                .FirstAsync();

            throw new NotEnoughPacksException(
                packName: packName,
                packsAmount: uPack.Amount - amount,
                packsNeeded: Math.Abs(amount)
            );
        }

        await _context.SaveChangesAsync();
        return uPack.Amount;
    }
}