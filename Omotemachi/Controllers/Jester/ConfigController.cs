using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Omotemachi.Models.V1.Jester.Config;
using Omotemachi.Services.Jester;

namespace Omotemachi.Controllers.Jester;
[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]")]
public class ConfigController(
    IConfigService<ExperienceConfig> expConfigService,
    IConfigService<ChannelsConfig> channelsConfigService,
    IConfigService<LogsConfig> logsConfigService,
    IConfigService<LootboxesConfig> lootboxesConfigService,
    IConfigService<RolesConfig> rolesConfigService,
    IConfigService<ShopConfig> shopConfigService,
    IConfigService<TicketsConfig> ticketsConfigService,
    IConfigService<VoiceConfig> voiceConfigService,
    IConfigService<EconomyConfig> economyConfigService,
    IConfigService<QuestsConfig> questsConfigService,
    IConfigService<PacksConfig> packsConfigService
) : ControllerBase
{
    private readonly IConfigService<ExperienceConfig> _expConfigService = expConfigService;
    private readonly IConfigService<ChannelsConfig> _channelsConfigService = channelsConfigService;
    private readonly IConfigService<LogsConfig> _logsConfigService = logsConfigService;
    private readonly IConfigService<LootboxesConfig> _lootboxesConfigService = lootboxesConfigService;
    private readonly IConfigService<RolesConfig> _rolesConfigService = rolesConfigService;
    private readonly IConfigService<ShopConfig> _shopConfigService = shopConfigService;
    private readonly IConfigService<TicketsConfig> _ticketsConfigService = ticketsConfigService;
    private readonly IConfigService<VoiceConfig> _voiceConfigService = voiceConfigService;
    private readonly IConfigService<EconomyConfig> _economyConfigService = economyConfigService;
    private readonly IConfigService<QuestsConfig> _questsConfigService = questsConfigService;
    private readonly IConfigService<PacksConfig> _packsConfigService = packsConfigService;

    [HttpGet("experience/{guildId}")]
    public async Task<IActionResult> ExperienceConfig(long guildId)
    {
        var config = await _expConfigService.GetOrCreateConfigAsync(guildId);
        return Ok(config);
    }
    [HttpPut("experience")]
    public async Task<IActionResult> ExperienceConfigSetUp(ExperienceConfig config)
    {
        await _expConfigService.UpdateConfigAsync(config);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("roles/{guildId}")]
    public async Task<IActionResult> RolesConfig(long guildId)
    {
        var config = await _rolesConfigService.GetOrCreateConfigAsync(guildId);
        return Ok(config);
    }
    [HttpPut("roles")]
    public async Task<IActionResult> RolesConfigSetUp(RolesConfig config)
    {
        await _rolesConfigService.UpdateConfigAsync(config);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("shop/{guildId}")]
    public async Task<IActionResult> ShopConfig(long guildId)
    {
        var config = await _shopConfigService.GetOrCreateConfigAsync(guildId);
        return Ok(config);
    }
    [HttpPut("shop")]
    public async Task<IActionResult> ShopConfigSetUp(ShopConfig config)
    {
        await _shopConfigService.UpdateConfigAsync(config);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("tickets/{guildId}")]
    public async Task<IActionResult> TicketsConfig(long guildId)
    {
        var config = await _ticketsConfigService.GetOrCreateConfigAsync(guildId);
        return Ok(config);
    }
    [HttpPut("tickets")]
    public async Task<IActionResult> TicketsConfigSetUp(TicketsConfig config)
    {
        await _ticketsConfigService.UpdateConfigAsync(config);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("voice/{guildId}")]
    public async Task<IActionResult> VoiceConfig(long guildId)
    {
        var config = await _voiceConfigService.GetOrCreateConfigAsync(guildId);
        return Ok(config);
    }
    [HttpPut("voice")]
    public async Task<IActionResult> VoiceConfigSetUp(VoiceConfig config)
    {
        await _voiceConfigService.UpdateConfigAsync(config);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("logs/{guildId}")]

    public async Task<IActionResult> LogsConfig(long guildId)
    {
        var config = await _logsConfigService.GetOrCreateConfigAsync(guildId);
        return Ok(config);
    }
    [HttpPut("logs")]
    public async Task<IActionResult> LogsConfigSetUp(LogsConfig config)
    {
        await _logsConfigService.UpdateConfigAsync(config);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("channels/{guildId}")]
    public async Task<IActionResult> ChannelsConfig(long guildId)
    {
        var config = await _channelsConfigService.GetOrCreateConfigAsync(guildId);
        return Ok(config);
    }
    [HttpPut("channels")]
    public async Task<IActionResult> ChannelsConfig(ChannelsConfig config)
    {
        await _channelsConfigService.UpdateConfigAsync(config);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("lootboxes/{guildId}")]
    public async Task<IActionResult> LootboxesConfig(long guildId)
    {
        var config = await _lootboxesConfigService.GetOrCreateConfigAsync(guildId);
        return Ok(config);
    }
    [HttpPut("lootboxes")]
    public async Task<IActionResult> LootboxesConfig(LootboxesConfig config)
    {
        await _lootboxesConfigService.UpdateConfigAsync(config);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("economy/{guildId}")]
    public async Task<IActionResult> EconomyConfig(long guildId)
    {
        var config = await _economyConfigService.GetOrCreateConfigAsync(guildId);
        return Ok(config);
    }
    [HttpPut("economy")]
    public async Task<IActionResult> EconomyConfig(EconomyConfig config)
    {
        await _economyConfigService.UpdateConfigAsync(config);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("quests/{guildId}")]
    public async Task<IActionResult> QuestsConfig(long guildId)
    {
        var config = await _questsConfigService.GetOrCreateConfigAsync(guildId);
        return Ok(config);
    }
    [HttpPut("quests")]
    public async Task<IActionResult> QuestsConfig(QuestsConfig config)
    {
        await _questsConfigService.UpdateConfigAsync(config);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("packs/{guildId}")]
    public async Task<IActionResult> PacksConfig(long guildId)
    {
        var config = await _packsConfigService.GetOrCreateConfigAsync(guildId);
        return Ok(config);
    }
    [HttpPut("packs")]
    public async Task<IActionResult> PacksConfig(PacksConfig config)
    {
        await _packsConfigService.UpdateConfigAsync(config);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
}