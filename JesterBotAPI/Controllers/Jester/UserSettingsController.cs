using Asp.Versioning;
using DellArteAPI.DTOS.V1.Jester;
using DellArteAPI.Exceptions.Jester.Settings;
using DellArteAPI.Models.V1.Jester.Settings;
using DellArteAPI.Services.Jester;
using Microsoft.AspNetCore.Mvc;

namespace DellArteAPI.Controllers.Jester;

[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]/{guildId}")]
public class UserSettingsController(IUserSettingsService settingsService) : ControllerBase
{
    private readonly IUserSettingsService _settingsService = settingsService;

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserSettings(long guildId, long userId)
    {
        try
        {
            return Ok(await _settingsService.GetPreparedUserSettingsDTO(guildId, userId));
        }
        catch(NoGuildSettingsAvailableException ex)
        {
            return BadRequest(new { ex.Code, ex.GuildId });
        }
    }

    [HttpPost("update/{type}")]
    public async Task<IActionResult> AddGuildSetting(long guildId, SettingTypes type, int? cost)
    {
        try
        {
            await _settingsService.AddGuildSetting(guildId, type, cost);
            return Ok(new { Success = true, Status = StatusCodes.Status200OK });
        }
        catch(GuildSettingAlreadyExistsException ex)
        {
            return BadRequest(new { ex.Code, ex.Type });
        }
    }
    [HttpDelete("update/{type}")]
    public async Task<IActionResult> RemoveGuildSetting(long guildId, SettingTypes type)
    {
        try
        {
            await _settingsService.RemoveGuildSetting(guildId, type);
            return Ok(new { Success = true, Status = StatusCodes.Status200OK });
        }
        catch(GuildSettingDoesNotExistsException ex) 
        {
            return BadRequest(new { ex.Code, ex.Type });
        }
    }
    [HttpPut("state/{userId}")]
    public async Task<IActionResult> UpdateSetting(long guildId, long userId, [FromBody] SettingDTO setting)
    {
        await _settingsService.UpdateUserSetting(guildId, userId, setting);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
}
