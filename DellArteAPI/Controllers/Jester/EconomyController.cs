using Asp.Versioning;
using DellArteAPI.Exceptions.Jester.Members;
using DellArteAPI.Models.V1;
using DellArteAPI.Services.Jester;
using Microsoft.AspNetCore.Mvc;

namespace DellArteAPI.Controllers.Jester;

[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]/{guildId}")]
public class EconomyController(IEconomyService economyService) : ControllerBase
{
    private readonly IEconomyService _economyService = economyService;

    [HttpPost("transactions/coins/{payerId}/{recipientId}")]
    public async Task<IActionResult> CreateCoinsTransaction(
        long guildId, long payerId, long recipientId, int amount)
    {
        try
        {
            var member = await _economyService.CreateCoinsTransaction(
                guildId, payerId, recipientId, amount);
            return Ok(member);
        }
        catch (NotEnoughCoinsException ex)
        {
            return BadRequest(new { ex.Code, guildId, ex.Current, ex.Needed });
        }
    }
    [HttpPost("transactions/crystals/{payerId}/{recipientId}")]
    public async Task<IActionResult> CreateCrystalsTransaction(
            long guildId, long payerId, long recipientId, int amount)
    {
        try
        {
            var member = await _economyService.CreateCrystalsTransaction(
                guildId, payerId, recipientId, amount);
            return Ok(member);
        }
        catch (NotEnoughCrystalsException ex)
        {
            return BadRequest(new { ex.Code, guildId, ex.Current, ex.Needed });
        }
    }
}
