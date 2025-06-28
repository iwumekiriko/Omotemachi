using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Omotemachi.Services.Wacky;
using Omotemachi.DTOS.V1.Wacky;
using Omotemachi.Exceptions.Wacky.CCG;

namespace Omotemachi.Controllers.Wacky;
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
    [HttpPost("cards/new/single")]
    public async Task<IActionResult> PostCard([FromBody] CardDTO card)
    {
        await _ccgService.CreateCardAsync(card);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpPost("cards/new/multiple")]
    public async Task<IActionResult> PostCards([FromBody] List<CardDTO> cards)
    {
        foreach (CardDTO card in cards) { await _ccgService.CreateCardAsync(card); }
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("cards/{guildId}/{userId}")]
    public async Task<IActionResult> GetUserCardCollection(long guildId, long userId)
    {
        return Ok(await _ccgService.GetUserCollection(guildId, userId));
    }
    [HttpGet("cards/random-asset")]
    public async Task<IActionResult> GetRandomCardAssetUrl()
    {
        return Ok(await _ccgService.GetRandomCardAssetUrl());
    }
    [HttpGet("cards/packs")]
    public async Task<IActionResult> GetAvailablePacks()
    {
        var packs = await _ccgService.GetAllAvailablePacks();
        return Ok(packs.ToDictionary(p => p.Id, pack => pack.Name));
    }
    [HttpPut("cards/packs/update/{guildId}/{userId}/{packId}")]
    public async Task<IActionResult> UpdatePacksCount(long guildId, long userId, int packId, int amount)
    {
        try
        {
            var left = await _ccgService.UpdatePacksCount(guildId, userId, packId, amount);
            return Ok(new { left });
        }
        catch (NotEnoughPacksException ex)
        {
            return BadRequest(new { ex.Code, ex.Name, ex.Amount, ex.Needed });
        }
    }
    [HttpPost("cards/packs/open/{guildId}/{userId}/{packId}")]
    public async Task<IActionResult> OpenCardPacks(long guildId, long userId, int packId, int amount = 1)
    {
        try
        {
            var drops = await _ccgService.GetCardsFromPack(guildId, userId, packId, amount);
            return Ok(drops);
        }
        catch (NotEnoughPacksException ex)
        {
            return BadRequest(new { ex.Code, ex.Name, ex.Amount, ex.Needed });
        }
    }

}