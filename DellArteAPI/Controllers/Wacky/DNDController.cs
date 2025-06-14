using Asp.Versioning;
using DellArteAPI.DTOS.V1;
using DellArteAPI.Services.Wacky;
using Microsoft.AspNetCore.Mvc;

namespace DellArteAPI.Controllers.Wacky;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]/{guildId}")]
public class DNDController(IDNDService DNDService) : ControllerBase
{
    private readonly IDNDService _DNDService = DNDService;

    [HttpPost("roll/{userId}")]
    public async Task<IActionResult> ProcessRollRequest(long guildId, long userId, [FromBody] List<RollResultDTO> results)
    {
        await _DNDService.ProcessRollResults(guildId, userId, results);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
}
