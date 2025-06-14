using Asp.Versioning;
using Humanizer;
using DellArteAPI.DTOS.V1.Jester;
using DellArteAPI.Exceptions.Jester.Inventory;
using DellArteAPI.Models.V1.Jester.Items;
using DellArteAPI.Models.V1.Jester.Lootboxes;
using DellArteAPI.Models.V1.Jester.Statistics;
using DellArteAPI.Services;
using DellArteAPI.Services.Jester;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace DellArteAPI.Controllers.Jester;
[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]/{guildId}/{userId}")]
public class InventoryController(
    IMembersService membersService,
    IInventoryService inventoryService,
    IStatisticsService statistics
) : ControllerBase
{
    private readonly IMembersService _membersService = membersService;
    private readonly IInventoryService _inventoryService = inventoryService;
    private readonly IStatisticsService _statistics = statistics;

    [HttpGet]
    public async Task<IActionResult> GetInventory(long guildId, long userId)
    {
        var inv = await _inventoryService.GetInventory(guildId, userId);

        var response = new
        {
            inventory_id = inv.InventoryId,
            guild = inv.Guild,
            user = inv.User,
            roles = inv.InventoryRoles.ToDictionary(
                ir => $"{ir.Role.GuildRoleId}",
                ir => new
                {
                    guildId,
                    guild = inv.Guild,
                    roleId = ir.RoleId,
                    guildRoleId = ir.Role.GuildRoleId,

                    quantity = 1
                }),
            expBoosters = inv.InventoryExpBoosters
                .GroupBy(ieb => new { ieb.ExpBooster.Value, ieb.ExpBooster.Duration })
                .ToDictionary(
                    ieb => $"x{ieb.First().ExpBooster.Value}_{ieb.First().ExpBooster.Duration}",
                    ieb => new
                    {
                        guildId,
                        guild = inv.Guild,
                        expBoosterId = ieb.First().ExpBoosterId,
                        value = ieb.First().ExpBooster.Value,
                        duration = ieb.First().ExpBooster.Duration,

                        quantity = ieb.First().Quantity
                    }
            ),
            lootboxKeys = inv.InventoryLootboxKeys
                .GroupBy(ilk => ilk.LootboxKey.Type)
                .ToDictionary(
                    ieb => $"{ieb.First().LootboxKey.Type}",
                    ieb => new
                    {
                        guildId,
                        guild = inv.Guild,
                        lootboxKeyId = ieb.First().LootboxKeyId,
                        type = ieb.First().LootboxKey.Type,

                        quantity = ieb.First().Quantity
                    }
            ),
        };

        return Ok(response);
    }
    [HttpPost("coins/add")]
    public async Task<IActionResult> AddCoins(long guildId, long userId, [FromBody] CoinDTO data)
    {
        var c = new Coin { Amount = data.Amount, GuildId = guildId };
        await _membersService.UpdateCoinsAsync(guildId, userId, data.Amount * data.Quantity);
        var response = new InventoryItemResponse<Coin>
        {
            Quantity = data.Quantity,
            Item = new Coin { Amount = data.Amount, GuildId = guildId }
        };
        return Ok(response);
    }
    [HttpGet("lootbox-keys/{type}")]
    public async Task<IActionResult> GetKeysCount(
        long guildId, long userId, LootboxTypes type)
    {
        var count = await _inventoryService.GetLootboxKeysCountAsync(guildId, userId, type);
        return Ok(count);
    }
    [HttpPut("lootbox-keys/{type}")]
    public async Task<IActionResult> UpdateLootboxKeysCount(
        long guildId, long userId, LootboxTypes type, int count = -1)
    {
        var guild = await _membersService.GetGuildAsync(guildId);
        var keyType = new LootboxKey { Guild = guild, Type = type };
        try
        {
            await _inventoryService.UpdateInventoryAsync(guildId, userId, keyType, count);
            if (count < 0)
            {
                Expression<Func<LootboxesStatistics, int>> propertySelector = type switch
                {
                    LootboxTypes.RolesLootbox => s => s.RolesLootboxesOpenedCount,
                    LootboxTypes.BackgroundsLootbox => s => s.BackgroundsLootboxesOpenedCount,
                    _ => throw new InvalidOperationException()
                };
                await _statistics.IncrementStatistics(guildId, userId, propertySelector, Math.Abs(count));
            }
        }
        catch(NotEnoughItemsException ex)
        {
            return BadRequest(new { ex.Code, ex.ItemType, ex.Current, ex.Needed });
        }
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpPost("lootbox-keys/add")]
    public async Task<IActionResult> AddLootboxKeys(long guildId, long userId, [FromBody] LootboxKeyDTO data)
    {
        var lk = new LootboxKey { Type = data.Type, GuildId = guildId };
        await _inventoryService.UpdateInventoryAsync(guildId, userId, lk, data.Quantity);
        var response = new InventoryItemResponse<LootboxKey>
        {
            Quantity = data.Quantity,
            Item = new LootboxKey { GuildId = guildId, Type = data.Type }
        };
        return Ok(response);
    }
    [HttpPost("exp-boosters/add")]
    public async Task<IActionResult> AddExpBoosters(long guildId, long userId, [FromBody] ExpBoosterDTO data)
    {
        var eb = new ExpBooster { Value = data.Value, Duration = data.Duration, GuildId = guildId };
        await _inventoryService.UpdateInventoryAsync(guildId, userId, eb, data.Quantity);
        var response = new InventoryItemResponse<ExpBooster>
        {
            Quantity = data.Quantity,
            Item = new ExpBooster { Duration = data.Duration, Value = data.Value, GuildId = guildId }
        };
        return Ok(response);
    }
    [HttpPost("exp-boosters/use")]
    public async Task<IActionResult> UseExpBooster(long guildId, long userId, [FromBody] ExpBoosterDTO data)
    {
        var eb = new ExpBooster { Value = data.Value, Duration = data.Duration, GuildId = guildId };
        try
        {
            var aeb = await _inventoryService.AddActiveBooster(guildId, userId, eb);
            await _membersService.SetExpMultiplier(guildId, userId, aeb.Value);
            return Ok(aeb);
        }
        catch (NotEnoughItemsException ex)
        {
            return BadRequest(new { ex.Code, ex.ItemType, ex.Current, ex.Needed });
        }
        catch (BoosterAlreadyActiveException ex)
        {
            return BadRequest(new { ex.Code, ex.Remaining });
        }
    }
    [HttpPost("exp-boosters/cancel")]
    public async Task<IActionResult> CancelActiveBooster(long guildId, long userId)
    {
        try
        {
            await _inventoryService.CancelActiveBooster(guildId, userId);
            await _membersService.SetExpMultiplier(guildId, userId, 1);
            return Ok(new { Success = true, Status = StatusCodes.Status200OK });
        }
        catch (NoActiveBoosterException ex)
        {
            return BadRequest(new { ex.Code });
        }
    }
    [HttpPost]
    [Route("/api/v{version:apiVersion}/[controller]/{guildId}/exp-boosters/reset")]
    public async Task<IActionResult> ResetActiveBoosters(long guildId)
    {
        var expiredBoosters = await _inventoryService.ResetActiveBoosters(guildId);
        var userIds = expiredBoosters.Select(pair => pair.Item2).ToList();

        var resetTasks = expiredBoosters
        .Select(pair => _membersService.SetExpMultiplier(
            pair.Item1,
            pair.Item2,
            1))
        .ToList();

        try
        {
            await Task.WhenAll(resetTasks)
                .WaitAsync(TimeSpan.FromSeconds(30));
        }
        catch (Exception)
        {
            //_logger.LogError(ex, "Failed to reset exp multipliers for guild {GuildId}", guildId);
            return StatusCode(500, "Partial reset completed");
        }
        return Ok(userIds);
    }
    [HttpGet("exp-boosters/active")]
    public async Task<IActionResult> GetActiveBooster(long guildId, long userId)
    {
        return Ok(await _inventoryService.GetActiveBooster(guildId, userId));
    }
    [HttpPost("roles/add")]
    public async Task<IActionResult> AddRole(long guildId, long userId, [FromBody] RoleDTO data)
    {
        var r = new Role { GuildRoleId = data.GuildRoleId, GuildId = guildId };
        try
        {
            await _inventoryService.UpdateInventoryAsync(guildId, userId, r, 1);
        }
        catch (AlreadyOwnsRoleException ex)
        {
            return BadRequest(new { ex.Code, ex.GuildRoleId });
        }
        var response = new InventoryItemResponse<Role>
        {
            Quantity = 1,
            Item = new Role { GuildRoleId = data.GuildRoleId, GuildId = guildId }
        };
        return Ok(response);
    }
    [HttpGet("roles/{guildRoleId}")]
    public async Task<IActionResult> MemberOwnsRole(long guildId, long userId, long guildRoleId)
    {
        return Ok(await _inventoryService.MemberOwnsRole(guildId, userId, guildRoleId));
    }
    [HttpGet("roles")]
    public async Task<IActionResult> GetMemberRoles(long guildId, long userId)
    {
        var roles = await _inventoryService.GetMemberRoles(guildId, userId);
        return Ok(roles.Select(ir => new
        {
            guildId,
            guild = ir.Inventory.Guild,
            roleId = ir.RoleId,
            guildRoleId = ir.Role.GuildRoleId,
            quantity = 1
        }).ToList());
    }
}