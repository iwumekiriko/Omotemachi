using DellArteAPI.DTOS.V1.Jester;
using DellArteAPI.Exceptions.Shop;
using DellArteAPI.Models.V1.Jester.Shop;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DellArteAPI.Services.Jester;

public interface IShopService
{
    List<ShopRole> GetShopRoles(long guildId);
    Task<List<ShopRole>> GetUserShopRoles(long guildId, long userId);
    List<ShopKey> GetShopKeys(long guildId);
    Task AddShopRole(ShopRole role);
    Task RemoveShopRole(ShopRole role);
    Task<ShopRoleTries> GetShopRoleTries(long guildId, long userId, long guildRoleId);
    Task TryShopRole(long guildId, long userId, long guildRoleId);
    Task<List<ExpiredRoleTryDTO>> ResetExpiredShopTries(long guildId);
    Task AddShopKey(ShopKey key);
    Task RemoveShopKey(ShopKey key);
}

public class ShopService(
    AppContext context,
    ILogger<ShopService> logger,
    IStatisticsService statistics
) : ServiceBase(context, logger), IShopService
{
    private readonly IStatisticsService _statistics = statistics;

    public List<ShopRole> GetShopRoles(long guildId)
    {
        return [.. _context.ShopRoles.Where(lr => lr.GuildId == guildId)];
    }
    public async Task<List<ShopRole>> GetUserShopRoles(long guildId, long userId)
    {
        var allRoles = GetShopRoles(guildId);
        var userRoleIds = await _context.InventoryRoles
            .Where(ir => ir.Inventory.UserId == userId && ir.Inventory.GuildId == guildId)
            .Select(ir => ir.Role.GuildRoleId)
            .ToListAsync();

        return [.. allRoles.Select(role =>
        {
            role.GotByUser = userRoleIds.Contains(role.GuildRoleId);
            return role;
        })];
    }
    public List<ShopKey> GetShopKeys(long guildId)
    {
        return [.. _context.ShopKeys.Where(lr => lr.GuildId == guildId)];
    }
    public async Task AddShopRole(ShopRole role)
    {
        var existingRole = await _context.ShopRoles
            .FirstOrDefaultAsync(
                lr => lr.GuildId == role.GuildId &&
                lr.GuildRoleId == role.GuildRoleId
        );

        if (existingRole == null)
        {
            _context.ShopRoles.Add(role);
            await _context.SaveChangesAsync();
        }
        else
            throw new ShopRoleAlreadyExistsException(
                guildRoleId: role.GuildRoleId
            );
    }
    public async Task RemoveShopRole(ShopRole role)
    {
        var existingRole = await _context.ShopRoles
            .FirstOrDefaultAsync(
                lr => lr.GuildId == role.GuildId &&
                lr.GuildRoleId == role.GuildRoleId
        );
        if (existingRole != null)
        {
            _context.ShopRoles.Remove(existingRole);
            await _context.SaveChangesAsync();
        }
        else
            throw new ShopRoleDoesNotExistException(
                guildRoleId: role.GuildRoleId
            );
    }
    public async Task<ShopRoleTries> GetShopRoleTries(long guildId, long userId, long guildRoleId)
    {
        var tryData = await _context.ShopRolesTries.
            FirstOrDefaultAsync(
                srt => srt.GuildId == guildId
                && srt.UserId == userId
                && srt.GuildRoleId == guildRoleId
            );
        if (tryData == null)
        {
            tryData = new ShopRoleTries {
                GuildId = guildId,
                UserId = userId,
                GuildRoleId = guildRoleId
            };
            _context.Add(tryData);
            await _context.SaveChangesAsync();
        }
        return tryData;
    }
    public async Task TryShopRole(long guildId, long userId, long guildRoleId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var tryData = await GetShopRoleTries(guildId, userId, guildRoleId);
            if (tryData.Active && tryData.TryActivated is { } tryActivated)
            {
                var willEndAt = DateTimeOffset.FromUnixTimeSeconds(tryActivated).AddMinutes(5);
                if (willEndAt > DateTimeOffset.UtcNow)
                    throw new LastTryDidntEndException(
                        willEndAt: willEndAt.ToUnixTimeSeconds()
                    );
            }
            if (tryData.TriesUsed >= 2)
                throw new AllShopTriesAreUsedException(
                    guildRoleId: guildRoleId
                );

            tryData.TriesUsed++;
            tryData.Active = true;
            tryData.TryActivated = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<List<ExpiredRoleTryDTO>> ResetExpiredShopTries(long guildId)
    {
        var activeTries = await _context.ShopRolesTries
            .Where(srt => srt.GuildId == guildId && srt.Active == true)
            .ToListAsync();

        List<ExpiredRoleTryDTO> usersWithExpiredTries = [];
        var currentTime = DateTimeOffset.UtcNow;

        foreach (var tryRecord in activeTries) {
            if (tryRecord.TryActivated is not long tryActivated)
                continue;

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(tryActivated).AddMinutes(5);
            if (expirationTime < currentTime)
            {
                tryRecord.Active = false;
                usersWithExpiredTries.Add(new ExpiredRoleTryDTO
                {
                    UserId = tryRecord.UserId,
                    GuildRoleId = tryRecord.GuildRoleId
                });
            }
        }
        await _context.SaveChangesAsync();
        return usersWithExpiredTries;
    }
    public async Task AddShopKey(ShopKey key)
    {
        var existingKey = await _context.ShopKeys
            .FirstOrDefaultAsync(
                lr => lr.GuildId == key.GuildId &&
                lr.LootboxType == key.LootboxType
        );

        if (existingKey == null)
        {
            _context.ShopKeys.Add(key);
            await _context.SaveChangesAsync();
        }
        else
            throw new ShopKeyAlreadyExistsException(
                lootboxType: key.LootboxType
            );
    }
    public async Task RemoveShopKey(ShopKey key)
    {
        var existingKey = await _context.ShopKeys
            .FirstOrDefaultAsync(
                lr => lr.GuildId == key.GuildId &&
                lr.LootboxType == key.LootboxType
        );
        if (existingKey != null)
        {
            _context.ShopKeys.Remove(existingKey);
            await _context.SaveChangesAsync();
        }
        else
            throw new ShopKeyDoesNotExistException(
                lootboxType: key.LootboxType
            );
    }
}
