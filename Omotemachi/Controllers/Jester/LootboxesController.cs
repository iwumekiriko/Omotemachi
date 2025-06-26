using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Omotemachi.Models.V1.Jester.Lootboxes;
using Omotemachi.Services.Jester;
using Omotemachi.Exceptions.Jester.Lootboxes;

namespace Omotemachi.Controllers.Jester;
[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]/{guildId}")]
public class LootboxesController(
    ILootboxesService lootboxesService
) : ControllerBase
{
    private readonly ILootboxesService _lootboxesService = lootboxesService;

    [HttpGet("roles/{lootboxType}")]
    public IActionResult GetLootboxRoles(long guildId, LootboxTypes lootboxType)
    {
        return Ok(_lootboxesService.GetLootboxRoles(guildId, lootboxType));
    }
    [HttpGet("roles/{lootboxType}/{userId}")]
    public async Task<IActionResult> GetUserLootboxRoles(long guildId, LootboxTypes lootboxType, long userId)
    {
        return Ok(await _lootboxesService.GetUserLootboxRoles(guildId, lootboxType, userId));
    }
    [HttpPost("roles/{lootboxType}/{guildRoleId}")]
    public async Task<IActionResult> AddLootboxRole(long guildId, LootboxTypes lootboxType, long guildRoleId, bool exclusive)
    {
        try
        {
            var role = new LootboxRole { GuildId = guildId, LootboxType = lootboxType, GuildRoleId = guildRoleId, Exclusive = exclusive };
            await _lootboxesService.AddLootboxRole(role);
        }
        catch(LootboxRoleAlreadyExistsException ex)
        {
            return BadRequest(new { ex.Code, ex.Type, ex.GuildRoleId });
        }
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpDelete("roles/{lootboxType}/{guildRoleId}")]
    public async Task<IActionResult> RemoveLootboxRole(long guildId, LootboxTypes lootboxType, long guildRoleId)
    {
        try
        {
            var role = new LootboxRole { GuildId = guildId, LootboxType = lootboxType, GuildRoleId = guildRoleId };
            await _lootboxesService.RemoveLootboxRole(role);
        }
        catch (LootboxRoleDoesNotExistException ex)
        {
            return BadRequest(new { ex.Code, ex.Type, ex.GuildRoleId });
        }
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("data/{userId}/{lootboxType}")]
    public async Task<IActionResult> GetLootboxUserData(long guildId, long userId, LootboxTypes lootboxType)
    {
        return Ok(await _lootboxesService.GetLootboxUserData(guildId, userId, lootboxType));
    }
    [HttpPost("data/{userId}/{lootboxType}")]
    public async Task<IActionResult> SaveLootboxUserData([FromBody] LootboxUserData userData)
    {
        await _lootboxesService.SaveLootboxUserData(userData);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
}