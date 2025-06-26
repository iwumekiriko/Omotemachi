using Microsoft.AspNetCore.Mvc;
using Omotemachi.Services;
using Omotemachi.Models.V1;
using Asp.Versioning;
using Omotemachi.Tools;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using Omotemachi.Models.V1.Jester.Top;
using Omotemachi.Services.Jester;
using Omotemachi.DTOS.V1.Jester;
using Omotemachi.Exceptions.Jester.Members;

namespace Omotemachi.Controllers.Jester;
[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]/{guildId}/{userId}")]
public class MembersController(
    IMembersService membersService,
    IQuestsService questsService,
    IImageRenderer renderer
) : ControllerBase
{
    private readonly IMembersService _membersService = membersService;
    private readonly IQuestsService _questsService = questsService;
    private readonly IImageRenderer _renderer = renderer;

    [HttpGet]
    public async Task<MemberDTO> Get(long guildId, long userId)
    {
        return await _membersService.GetMemberDTOAsync(guildId, userId);
    }
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(
        long guildId, long userId,  
        string avatar_url, string nickname)
    {
        var member = await _membersService.GetMemberAsync(1299668096474157076, 567303956448018456);

        var nicknameElement = new TextWithBackgroundElement
        {
            Offset = new PointF(0, 500),
            Text = nickname,
            FontSize = 150,
            Anchor = AnchorPosition.Center,
            TextHorizontalAlignment = HorizontalAlignment.Center,
            TextVerticalAlignment = VerticalAlignment.Center,
            Padding = 20,
            BackgroundColor = Color.White.WithAlpha(0.5f),
            TextColor = Color.White,
            CornerRadius = 20,
        };
        var voiceInfoElement = new TextWithBackgroundElement
        {
            Anchor = AnchorPosition.CenterLeft,
            Offset = new PointF(750, 0),
            //Text = member.VoiceTime.ToString(),
            TextHorizontalAlignment = HorizontalAlignment.Center,
            TextVerticalAlignment = VerticalAlignment.Center,
            Padding = 20,
            FontSize = 150,
            BackgroundColor = Color.White.WithAlpha(0.5f),
            TextColor = Color.White,
            CornerRadius = 20
        };
        var avatarElement = new ImageElement
        {
            Anchor = AnchorPosition.Center,
            Offset = new PointF(0, -500),
            ImageUrl = "https://i.imgur.com/xKICzua.jpeg",
            CornerRadius = 50,
        };

        var elements = new List<CanvasElement>
        {
            nicknameElement,
            voiceInfoElement,
            avatarElement
        };
        return File(
            await _renderer.RenderToStreamAsync(
                "https://i.imgur.com/vQvRYX6.png",
                elements,
                SixLabors.ImageSharp.Formats.Png.PngFormat.Instance),
            "image/png");
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
