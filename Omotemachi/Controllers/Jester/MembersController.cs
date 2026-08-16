using Microsoft.AspNetCore.Mvc;
using Omotemachi.Services;
using Asp.Versioning;
using Omotemachi.Services.Jester;
using Omotemachi.Exceptions.Jester.Members;
using Omotemachi.Models.V1.Domain;
using Omotemachi.Models.V1.Domain.Jester.Top;
using Omotemachi.Models.V1.DTOs.Jester;

namespace Omotemachi.Controllers.Jester;
[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]/{guildId}/{userId}")]
public class MembersController(
    IMembersService membersService,
    IQuestsService questsService
) : ControllerBase
{
    private readonly IMembersService _membersService = membersService;
    private readonly IQuestsService _questsService = questsService;

    [HttpGet]
    public async Task<MemberDTO> Get(long guildId, long userId)
    {
        return await _membersService.GetMemberDTOAsync(guildId, userId);
    }

    [HttpPut("join")]
    public async Task<IActionResult> Join(long guildId, long userId)
    {
        await _membersService.OnMemberJoin(guildId, userId);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpPut("leave")]
    public async Task<IActionResult> Leave(long guildId, long userId)
    {
        await _membersService.OnMemberLeave(guildId, userId);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpPut("message")]
    public async Task<IActionResult> Message(long guildId, long userId, long channelId)
    {
        var member = await _membersService.HandleMessage(guildId, userId);
        await _questsService.AddProgress(guildId, userId, channelId, 1);
        return Ok(member);
    }
    [HttpPut("voice")]
    public async Task<IActionResult> Voice(
        long guildId,
        long userId,
        int seconds,
        long channelId,
        bool muted
    )
    {
        var (member, minutes) = await _membersService.HandleVoice(guildId, userId, seconds, muted);
        await _questsService.AddProgress(guildId, userId, channelId, minutes);
        return Ok(member);
    }
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] Member member)
    {
        await _membersService.MemberUpdate(member);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [ApiVersion(1, Deprecated = true)]
    [HttpPut("coins")]
    public async Task<IActionResult> Coins(long guildId, long userId, int amount)
    {
        try
        {
            var member = await _membersService.UpdateCoinsAsync(guildId, userId, amount);
            return Ok(member);
        }
        catch (NotEnoughCoinsException ex)
        {
            return BadRequest(new { ex.Code, guildId, ex.Current, ex.Needed });
        }
    }
    [HttpGet("top/{type}")]
    public async Task<IActionResult> GetTop(long guildId, long userId, TopTypes type)
    {
        return Ok(await _membersService.GetTop(guildId, userId, type));
    }
}
