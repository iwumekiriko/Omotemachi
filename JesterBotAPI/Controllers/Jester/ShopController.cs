using Asp.Versioning;
using DellArteAPI.Exceptions.Jester.Inventory;
using DellArteAPI.Exceptions.Shop;
using DellArteAPI.Models.V1.Jester.Lootboxes;
using DellArteAPI.Models.V1.Jester.Shop;
using DellArteAPI.Services.Jester;
using Microsoft.AspNetCore.Mvc;

namespace DellArteAPI.Controllers.Jester;

[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]/{guildId}")]
public class ShopController(
    IInventoryService inventoryService, IShopService shopService
) : ControllerBase
{
    private readonly IShopService _shopService = shopService;
    private readonly IInventoryService _inventoryService = inventoryService;

    [HttpGet("roles")]
    public IActionResult GetShopRoles(long guildId)
    {
        return Ok(_shopService.GetShopRoles(guildId));
    }
    [HttpGet("roles/{userId}")]
    public async Task<IActionResult> GetUserShopRoles(long guildId, long userId)
    {
        return Ok(await _shopService.GetUserShopRoles(guildId, userId));
    }
    [HttpGet("keys")]
    public IActionResult GetShopKeys(long guildId)
    {
        return Ok(_shopService.GetShopKeys(guildId));
    }
    [HttpPost("roles/{guildRoleId}")]
    public async Task<IActionResult> AddShopRole(long guildId, long guildRoleId, bool exclusive, int price)
    {
        try
        {
            var role = new ShopRole {
                GuildId = guildId,
                GuildRoleId = guildRoleId,
                Exclusive = exclusive,
                Price = price 
            };
            await _shopService.AddShopRole(role);
        }
        catch (ShopRoleAlreadyExistsException ex)
        {
            return BadRequest(new { ex.Code, ex.GuildRoleId });
        }
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpDelete("roles/{guildRoleId}")]
    public async Task<IActionResult> RemoveShopRole(long guildId, long guildRoleId)
    {
        try
        {
            var role = new ShopRole { GuildId = guildId, GuildRoleId = guildRoleId };
            await _shopService.RemoveShopRole(role);
        }
        catch (ShopRoleDoesNotExistException ex)
        {
            return BadRequest(new { ex.Code, ex.GuildRoleId });
        }
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("roles/{userId}/{guildRoleId}/try")]
    public async Task<IActionResult> GetShopRoleTries(long guildId, long userId, long guildRoleId)
    {
        return Ok(await _shopService.GetShopRoleTries(guildId, userId, guildRoleId));
    }
    [HttpPut("roles/{userId}/{guildRoleId}/try")]
    public async Task<IActionResult> TryShopRole(long guildId, long userId, long guildRoleId)
    {
        if (await _inventoryService.MemberOwnsRole(guildId, userId, guildRoleId))
        {
            var ex = new AlreadyOwnsRoleException(guildRoleId);
            return BadRequest(new { ex.Code, ex.GuildRoleId });
        }
        try
        {
            await _shopService.TryShopRole(guildId, userId, guildRoleId);
            return Ok(new { Success = true, Status = StatusCodes.Status200OK });
        }
        catch (AllShopTriesAreUsedException ex)
        {
            return BadRequest(new { ex.Code, ex.GuildRoleId });
        }
        catch (LastTryDidntEndException ex)
        {
            return BadRequest(new { ex.Code, ex.WillEndAt });
        }
    }
    [HttpPost("roles/tries/reset")]
    public async Task<IActionResult> ResetExpiredShopTries(long guildId)
    {
        return Ok(await _shopService.ResetExpiredShopTries(guildId));
    }
    [HttpPost("keys/{lootboxType}")]
    public async Task<IActionResult> AddShopKey(long guildId, LootboxTypes lootboxType, bool exclusive)
    {
        try
        {
            var key = new ShopKey { GuildId = guildId, LootboxType = lootboxType, Exclusive = exclusive };
            await _shopService.AddShopKey(key);
        }
        catch (ShopKeyAlreadyExistsException ex)
        {
            return BadRequest(new { ex.Code, ex.LootboxType });
        }
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpDelete("keys/{lootboxType}")]
    public async Task<IActionResult> RemoveShopKey(long guildId, LootboxTypes lootboxType)
    {
        try
        {
            var key = new ShopKey { GuildId = guildId, LootboxType = lootboxType };
            await _shopService.RemoveShopKey(key);
        }
        catch (ShopKeyDoesNotExistException ex)
        {
            return BadRequest(new { ex.Code, ex.LootboxType });
        }
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
}
