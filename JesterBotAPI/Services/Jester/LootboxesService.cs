using DellArteAPI.Exceptions.Jester.Lootboxes;
using DellArteAPI.Models.V1.Jester.Lootboxes;
using DellArteAPI.Models.V1.Jester.Statistics;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DellArteAPI.Services.Jester;

public interface ILootboxesService
{
    List<LootboxRole> GetLootboxRoles(long guildId, LootboxTypes type);
    Task<List<LootboxRole>> GetUserLootboxRoles(long guildId, LootboxTypes lootboxType, long userId);
    Task AddLootboxRole(LootboxRole role);
    Task RemoveLootboxRole(LootboxRole role);
    Task<LootboxUserData> GetLootboxUserData(long guildId, long userId, LootboxTypes type);
    Task SaveLootboxUserData(LootboxUserData userData);
}

public class LootboxesService(
    AppContext context,
    ILogger<LootboxesService> logger,
    IStatisticsService statistics
) : ServiceBase(context, logger), ILootboxesService
{
    private readonly IStatisticsService _statistics = statistics;

    public List<LootboxRole> GetLootboxRoles(long guildId, LootboxTypes type)
    {
        return [.. _context.LootboxRoles.Where(lr => lr.GuildId == guildId && lr.LootboxType == type)];
    }
    public async Task<List<LootboxRole>> GetUserLootboxRoles(long guildId, LootboxTypes lootboxType, long userId)
    {
        var allRoles = GetLootboxRoles(guildId, lootboxType);
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
    public async Task AddLootboxRole(LootboxRole role)
    {
        var existingRole = await _context.LootboxRoles
            .FirstOrDefaultAsync(
                lr => lr.GuildId == role.GuildId &&
                lr.LootboxType == role.LootboxType &&
                lr.GuildRoleId == role.GuildRoleId
        );

        if (existingRole == null)
        {
            _context.LootboxRoles.Add(role);
            await _context.SaveChangesAsync();
        }
        else
            throw new LootboxRoleAlreadyExistsException(
                type: role.LootboxType,
                guildRoleId: role.GuildRoleId
            );
    }
    public async Task RemoveLootboxRole(LootboxRole role)
    {
        var existingRole = await _context.LootboxRoles
            .FirstOrDefaultAsync(
                lr => lr.GuildId == role.GuildId &&
                lr.LootboxType == role.LootboxType &&
                lr.GuildRoleId == role.GuildRoleId
        );
        if (existingRole != null)
        {
            _context.LootboxRoles.Remove(existingRole);
            await _context.SaveChangesAsync();
        }
        else
            throw new LootboxRoleDoesNotExistException(
                type: role.LootboxType,
                guildRoleId: role.GuildRoleId
            );
    }
    public async Task<LootboxUserData> GetLootboxUserData(long guildId, long userId, LootboxTypes type)
    {
        var data = await _context.LootboxUserDatas.FirstOrDefaultAsync(
            lud => lud.GuildId == guildId && lud.UserId == userId && lud.LootboxType == type
        );
        if (data == null)
        {
            data = new LootboxUserData
            {
                UserId = userId,
                GuildId = guildId,
                LootboxType = type,
            };
            _context.LootboxUserDatas.Add(data);
            await _context.SaveChangesAsync();
        }
        return data;
    }
    public async Task SaveLootboxUserData(LootboxUserData userData)
    {
        var data = await _context.LootboxUserDatas.FirstOrDefaultAsync(
            lud => lud.GuildId == userData.GuildId && lud.UserId == userData.UserId && lud.LootboxType == userData.LootboxType
        );
        if (data == null)
        {
            data = new LootboxUserData
            {
                UserId = userData.UserId,
                GuildId = userData.GuildId,
                LootboxType = userData.LootboxType,
            };
            _context.LootboxUserDatas.Add(data);
        }
        data.Data = userData.Data;

        if (data.Data.TryGetValue("rolesAttempts", out int _))
        {
            await _statistics.SetStatistics<LootboxesStatistics>(
                userData.GuildId,
                userData.UserId,
                s => s.RolesLootboxesOpenedCount,
                userData.Data["totalAttempts"]
            );
            await _statistics.SetStatistics<LootboxesStatistics>(
                userData.GuildId,
                userData.UserId,
                s => s.LootboxesRolesDroppedCount,
                userData.Data["rolesGot"]
            );
        }

        if (data.Data.TryGetValue("backgroundsAttempts", out int _))
        {
            await _statistics.SetStatistics<LootboxesStatistics>(
                userData.GuildId,
                userData.UserId,
                s => s.BackgroundsLootboxesOpenedCount,
                userData.Data["totalAttempts"]
            );
            await _statistics.SetStatistics<LootboxesStatistics>(
                userData.GuildId,
                userData.UserId,
                s => s.LootboxesBackgroundsDroppedCount,
                userData.Data["backgroundsGot"]
            );
        }

        await _context.SaveChangesAsync();
    }
}
