using Asp.Versioning;
using DellArteAPI.Exceptions.Jester.Duets;
using DellArteAPI.Services.Jester;
using Microsoft.AspNetCore.Mvc;
namespace DellArteAPI.Controllers.Jester;

[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]/{guildId}")]
public class DuetsController(IDuetsService duetsService) : ControllerBase
{
    private readonly IDuetsService _duetsService = duetsService;

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetDuet(long guildId, long userId)
    {
        var duet = await _duetsService.GetDuet(guildId, userId);

        if (duet == null)
            return NoContent();

        return Ok(duet);
    }
    [HttpPost("{proposerId}/{duoId}")]
    public async Task<IActionResult> CreateDuet(long guildId, long proposerId, long duoId)
    {
        await _duetsService.CreateDuet(guildId, proposerId, duoId);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteDuet(long guildId, long userId)
    {
        try
        {
            await _duetsService.DeleteDuet(guildId, userId);
            return Ok(new { Success = true, Status = StatusCodes.Status200OK });
        }
        catch (DisposeRestrictedException ex)
        {
            return BadRequest(new { ex.Code });
        }
    }
}