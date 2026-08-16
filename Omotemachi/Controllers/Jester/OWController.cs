using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Omotemachi.Models.V1.Domain.Jester.OW;
using Omotemachi.Services.Jester;

namespace Omotemachi.Controllers.Jester;

[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]/{userId}")]
public class OWController(
    IOWService owService
) : ControllerBase
{
    private readonly IOWService _owService = owService;

    [HttpGet]
    public async Task<UserHeroProgress> GetUserHeroProgressAsync(long userId)
    {
        return await _owService.GetOrCreateAsync(userId);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUserHeroProgressAsync(long userId, string role, List<string> heroes)
    {
        await _owService.UpdateHeroesAsync(userId, role, heroes);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetUserHeroProgressAsync(long userId)
    {
        await _owService.ResetAllHeroesAsync(userId);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
}
