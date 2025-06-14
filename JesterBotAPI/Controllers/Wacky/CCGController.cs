using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using DellArteAPI.Services.Wacky;
using DellArteAPI.DTOS.V1.Wacky;

namespace DellArteAPI.Controllers.Wacky;
[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]")]
public class CCGController(ICCGService ccgService) : ControllerBase
{
    private readonly ICCGService _ccgService = ccgService;

    [HttpGet("series")]
    public async Task<IActionResult> GetSeries()
    {
        return Ok(await _ccgService.GetSeriesAsync());
    }
    [HttpPost("series")]
    public async Task<IActionResult> CreateSeries(string name)
    {
        await _ccgService.CreateSeriesAsync(name);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("series/{id:int}")]
    public async Task<IActionResult> GetSeriesById(int id)
    {
        return Ok(await _ccgService.GetSeriesByIdAsync(id));
    }
    [HttpGet("series/{name}")]
    public async Task<IActionResult> GetSeriesByName(string name)
    {
        return Ok(await _ccgService.GetSeriesByNameAsync(name));
    }
    [HttpGet("cards")]
    public async Task<IActionResult> GetCards()
    {
        return Ok(await _ccgService.GetAllCardsAsync());
    }
    [HttpPost("cards")]
    public async Task<IActionResult> PostCard([FromBody] CardDTO card)
    {
        await _ccgService.CreateCardAsync(card);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpPost("cards/pack/{guildId}/{userId}")]
    public async Task<IActionResult> OpenCardPacks(long guildId, long userId, int amount = 1)
    {
        var drops = await _ccgService.OpenCardsPacks(guildId, userId, amount);
        return Ok(drops);
    }

}