using DellArteAPI.Exceptions.Jester.Inventory;
using DellArteAPI.Models.V1;
using DellArteAPI.Models.V1.Jester;
using DellArteAPI.Models.V1.Jester.InventoryItems;
using DellArteAPI.Models.V1.Jester.Items;
using DellArteAPI.Models.V1.Jester.Lootboxes;
using DellArteAPI.Models.V1.Jester.Settings;
using DellArteAPI.Models.V1.Jester.Statistics;
using Microsoft.EntityFrameworkCore;

namespace DellArteAPI.Services.Jester;

public interface IInventoryService
{
    Task<Inventory> GetInventory(long guildId, long userId);
    Task<int> GetLootboxKeysCountAsync(long guildId, long userId, LootboxTypes type);
    Task UpdateInventoryAsync(long guildId, long userId, Item item, int quantity);
    Task<ActiveExpBooster> AddActiveBooster(long guildId, long userId, ExpBooster eb);
    Task CancelActiveBooster(long guildId, long userId);
    Task<List<(long, long)>> ResetActiveBoosters(long guildId);
    Task<ActiveExpBooster?> GetActiveBooster(long guildId, long userId);
    Task<bool> MemberOwnsRole(long guildId, long userId, long guildRoleId);
    Task<ICollection<InventoryRole>> GetMemberRoles(long guildId, long userId);
}

public class InventoryService(
    AppContext context,
    ILogger<InventoryService> logger,
    IStatisticsService statistics,
    IUserSettingsService uSettingService
) : ServiceBase(context, logger), IInventoryService
{
    private readonly IStatisticsService _statistics = statistics;
    private readonly IUserSettingsService _uSettingService = uSettingService;

    public async Task<Inventory> GetInventory(long guildId, long userId)
    {
        var inventory = await _context.Inventories
            .Include(i => i.Guild)
            .Include(i => i.User)
            .Include(i => i.InventoryRoles)
                .ThenInclude(ir => ir.Role)
            .Include(i => i.InventoryExpBoosters)
                .ThenInclude(ieb => ieb.ExpBooster)
            .Include(i => i.InventoryLootboxKeys)
                .ThenInclude(ilk => ilk.LootboxKey)
            .AsSplitQuery()
            .FirstOrDefaultAsync(i => i.GuildId == guildId &&
                                      i.UserId == userId);

        if (inventory == null)
        {
            var guild = _context.Guilds.FirstOrDefault(g => g.Id == guildId) ?? new Guild { Id = guildId };
            var user = _context.Users.FirstOrDefault(u => u.Id == userId) ?? new User { Id = userId };
            inventory = new Inventory
            {
                Guild = guild,
                User = user
            };
            _context.Add(inventory);
            await _context.SaveChangesAsync();
        }
        return inventory;
    }
    public async Task<int> GetLootboxKeysCountAsync(long guildId, long userId, LootboxTypes type)
    {
        var inventory = await _context.Inventories
            .Include(i => i.InventoryLootboxKeys)
            .ThenInclude(ilk => ilk.LootboxKey)
            .FirstOrDefaultAsync(i => i.GuildId == guildId && i.UserId == userId);

        return inventory?.InventoryLootboxKeys
            .FirstOrDefault(ilk => ilk.LootboxKey.Type == type)?
            .Quantity ?? 0;
    }
    public async Task UpdateInventoryAsync(long guildId, long userId, Item item, int quantity)
    {
        var inventory = await GetInventory(guildId, userId);
        if (quantity == 0)
            throw new ArgumentException("0 quantity");

        switch (item)
        {
            case ExpBooster expBooster:
                UpdateExpBoostersAsync(inventory, expBooster, quantity);
                break;

            case LootboxKey lootboxKey:
                UpdateLootboxKeysAsync(inventory, lootboxKey, quantity);
                break;

            case Role role:
                UpdateRolesAsync(inventory, role, quantity);
                await MarkActiveTryAsInactiveAsync(guildId, userId, role.GuildRoleId);
                break;

            default:
                throw new ArgumentException("item type does not match");
        }

        await _context.SaveChangesAsync();
    }
    private static void UpdateExpBoostersAsync(Inventory inv, ExpBooster expBooster, int quantity)
    {
        var existingBooster = inv.InventoryExpBoosters
            .FirstOrDefault(ieb => ieb.ExpBooster.Value == expBooster.Value &&
                                   ieb.ExpBooster.Duration == expBooster.Duration);

        if (existingBooster != null)
        {
            existingBooster.Quantity += quantity;

            if (existingBooster.Quantity < 0)
                throw new NotEnoughItemsException(
                    current: existingBooster.Quantity - quantity,
                    needed: Math.Abs(quantity),
                    itemType: "ExpBooster"
                );
        }
        else if (quantity > 0)
        {
            inv.InventoryExpBoosters.Add(new InventoryExpBooster
            {
                ExpBooster = expBooster,
                Quantity = quantity
            });
        }
        else
            throw new InvalidOperationException();
    }
    private static void UpdateLootboxKeysAsync(Inventory inv, LootboxKey lootboxKey, int quantity)
    {
        var existingKey = inv.InventoryLootboxKeys
            .FirstOrDefault(ilk => ilk.LootboxKey.Type == lootboxKey.Type);

        if (existingKey != null)
        {
            existingKey.Quantity += quantity;

            if (existingKey.Quantity < 0)
                throw new NotEnoughItemsException(
                    current: existingKey.Quantity - quantity,
                    needed: Math.Abs(quantity),
                    itemType: "LootboxKey"
                );
        }
        else if (quantity > 0)
        {
            inv.InventoryLootboxKeys.Add(new InventoryLootboxKey
            {
                LootboxKey = lootboxKey,
                Quantity = quantity
            });
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
    private static void UpdateRolesAsync(Inventory inv, Role role, int quantity)
    {
        var existingRole = inv.InventoryRoles
            .FirstOrDefault(ir => ir.Role.GuildRoleId == role.GuildRoleId);

        if (existingRole != null)
        {
            if (quantity < 0)
            {
                inv.InventoryRoles.Remove(existingRole);
            }

            else
                throw new AlreadyOwnsRoleException(guildRoleId: role.GuildRoleId);
        }
        else
        {
            if (quantity > 0)
            {
                inv.InventoryRoles.Add(new InventoryRole
                {
                    Role = role
                });
            }

            else
                throw new InvalidOperationException();
        }
    }
    private async Task MarkActiveTryAsInactiveAsync(long guildId, long userId, long guildRoleId)
    {
        var activeTry = await _context.ShopRolesTries
            .FirstOrDefaultAsync(srt =>
                srt.GuildId == guildId &&
                srt.UserId == userId &&
                srt.GuildRoleId == guildRoleId &&
                srt.Active
        );
        if (activeTry != null)
            activeTry.Active = false;
    }
    public async Task<ActiveExpBooster> AddActiveBooster(long guildId, long userId, ExpBooster eb)
    {
        var existingBooster = await _context.ActiveExpBoosters.FirstOrDefaultAsync(
            aeb => aeb.UserId == userId && aeb.GuildId == guildId);

        if (existingBooster != null)
        {
            long currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long boosterEndTime = existingBooster.ActivatedAt + existingBooster.Duration;
            int remainingSeconds = (int)(boosterEndTime - currentUnixTime);

            throw new BoosterAlreadyActiveException(
               remaining: remainingSeconds
            );
        }
        
        await UpdateInventoryAsync(guildId, userId, eb, -1);
        var aeb = new ActiveExpBooster
        {
            ActivatedAt = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
            Duration = eb.Duration,
            Value = eb.Value,
            GuildId = guildId,
            UserId = userId,
        };
        _context.ActiveExpBoosters.Add(aeb);

        await _statistics.IncrementStatistics<InventoryStatistics>(
            guildId, userId, s => s.ExpBoostersActivatedCount);

        await _context.SaveChangesAsync();
        return aeb;
    }
    public async Task CancelActiveBooster(long guildId, long userId)
    {
        var activeBooster = await GetActiveBooster(guildId, userId);

        if (activeBooster == null)
        {
            throw new NoActiveBoosterException();
        }
        else
        {
            _context.ActiveExpBoosters.Remove(activeBooster);
        }
        await _context.SaveChangesAsync();
    }
    public async Task<List<(long, long)>> ResetActiveBoosters(long guildId)
    {
        var currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var expiredBoosters = await _context.ActiveExpBoosters
            .Where(
                aeb => aeb.ActivatedAt + aeb.Duration < currentUnixTime
                && aeb.GuildId == guildId)
            .ToListAsync();

        var expiredPairs = expiredBoosters
            .Select(b => (b.GuildId, b.UserId))
            .ToList();

        _context.ActiveExpBoosters.RemoveRange(expiredBoosters);
        await _context.SaveChangesAsync();

        //
        // extending boosters after removing from db
        //
        foreach (var booster in expiredBoosters)
            await TryExtendBooster(booster);

        return expiredPairs;
    }
    public async Task TryExtendBooster(ActiveExpBooster lastBooster)
    {
        var guildId = lastBooster.GuildId;
        var userId = lastBooster.UserId;

        var uSetting = await _uSettingService.GetUserSetting(guildId, userId, SettingTypes.AutoBoostsExtend);
        if (uSetting?.State != true)
            return;

        var userBoosters = await _context.InventoryExpBoosters
        .Where(ieb =>
            ieb.Inventory.GuildId == guildId &&
            ieb.Inventory.UserId == userId &&
            ieb.ExpBooster != null)
        .Select(ieb => new
        {
            Booster = ieb.ExpBooster,
            InventoryItem = ieb
        })
        .ToListAsync();

        var matchingBooster = userBoosters
            .FirstOrDefault(x => x.Booster.Value == lastBooster.Value)?.Booster;

        if (matchingBooster != null)
        {
            await AddActiveBooster(guildId, userId, matchingBooster);
            return;
        }

        var fallbackBooster = userBoosters.FirstOrDefault()?.Booster;
        if (fallbackBooster != null)
        {
            await AddActiveBooster(guildId, userId, fallbackBooster);
        }
    }
    public async Task<ActiveExpBooster?> GetActiveBooster(long guildId, long userId)
    {
        var activeBooster = await _context.ActiveExpBoosters.FirstOrDefaultAsync(
            aeb => aeb.GuildId == guildId && aeb.UserId == userId
        );
        return activeBooster;
    }
    public async Task<bool> MemberOwnsRole(long guildId, long userId, long guildRoleId)
    {
        var inv = await _context.Inventories
            .Include(i => i.InventoryRoles)
                .ThenInclude(ir => ir.Role)
            .FirstOrDefaultAsync(i => i.GuildId == guildId &&
                                      i.UserId == userId);

        if (inv == null) return false;
        return inv.InventoryRoles.FirstOrDefault(ir => ir.Role.GuildRoleId == guildRoleId) != null;
    }
    public async Task<ICollection<InventoryRole>> GetMemberRoles(long guildId, long userId)
    {
        return await _context.InventoryRoles
            .Include(ir => ir.Role)
            .Include(ir => ir.Inventory)
            .Where(ir => ir.Inventory.GuildId == guildId &&
                        ir.Inventory.UserId == userId)
            .ToListAsync();
    }
}