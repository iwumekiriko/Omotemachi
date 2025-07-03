using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Omotemachi.Services.Wacky;
using Omotemachi.Models.V1.Wacky.Appa;
using Omotemachi.Exceptions.Wacky;

namespace Omotemachi.Controllers.Wacky;
[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]")]
public class AppaController(IAppaService appaService) : ControllerBase
{
    private readonly IAppaService _appaService = appaService;

    [HttpPost]
    public async Task<IActionResult> PostAppa([FromBody] Appa appa)
    {
        await _appaService.AddAppaAsync(appa);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpPost("manual")]
    public async Task<IActionResult> PostAppa(string name, string assetUrl)
    {
        var appa = new Appa { Name = name, AssetUrl = assetUrl };
        await _appaService.AddAppaAsync(appa);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAppa(int id)
    {
        return Ok(await _appaService.GetAppaByIdAsync(id));
    }
    [HttpGet("{name}")]
    public async Task<IActionResult> GetAppa(string name)
    {
        return Ok(await _appaService.GetAppaByNameAsync(name));
    }
    [HttpGet("{guildId}/{userId}/random")]
    public async Task<IActionResult> GetRandomAppa(long guildId, long userId)
    {
        try
        {
            return Ok(await _appaService.GetAndCountRandomUserAppaAsync(guildId, userId));
        }
        catch (CommandTimeoutException ex)
        {
            return BadRequest(new { ex.Code, ex.TimeLeft });
        }
    }
    [HttpGet("{guildId}/{userId}/collection")]
    public async Task<IActionResult> GetUserAppasCollection(long guildId, long userId)
    {
        return Ok(await _appaService.GetUserAppasAsync(guildId, userId));
    }
}
