using Asp.Versioning;
using DellArteAPI.Exceptions.Jester.Interactions;
using DellArteAPI.Models.V1.Jester.Interactions;
using DellArteAPI.Services.Jester;
using Microsoft.AspNetCore.Mvc;

namespace DellArteAPI.Controllers.Jester;

[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]/{guildId}")]
public class InteractionsController(IInteractionsService interactionsService) : ControllerBase
{
    private readonly IInteractionsService _interactionsService = interactionsService;

    [HttpGet("{targetId}/is-restricted")]
    public async Task<IActionResult> IsTargetInteractionRestricted(long guildId, long targetId)
    {
        return Ok(await _interactionsService.IsInteractionRestricted(guildId, targetId));
    }

    [HttpPut("upload")]
    public async Task<IActionResult> UploadGifs(long guildId, [FromBody] List<InteractionsAsset> assets)
    {
        await _interactionsService.UploadGifsAsync(guildId, assets);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }

    [HttpGet("{iAction}/{iType}")]
    public async Task<IActionResult> GetGif(long guildId, InteractionsActions iAction, InteractionsTypes iType)
    {
        try
        {
            return Ok(new { Success = true, (await _interactionsService.GetAssetAsync(guildId, iAction, iType)).AssetUrl});
        }
        catch (NoAvailableGifsException ex)
        {
            return BadRequest(new { ex.Code, ex.Action, ex.Type });
        }
    }
}
