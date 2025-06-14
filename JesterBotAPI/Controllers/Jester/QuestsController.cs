using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using DellArteAPI.Models.V1.Jester.Quests;
using DellArteAPI.Services.Jester;
using DellArteAPI.DTOS.V1.Jester.Quests;
using DellArteAPI.Exceptions.Jester.Quests;

namespace DellArteAPI.Controllers.Jester;
[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]/{guildId}")]
public class QuestsController(IQuestsService questsService) : ControllerBase
{
    private readonly IQuestsService _questsService = questsService;

    [HttpPost("templates")]
    public async Task<IActionResult> AddQuest([FromBody] QuestTemplateDTO questTemplate)
    {
        var quest = new Quest { 
            GuildId = questTemplate.GuildId,
            Type = questTemplate.Type,
            TaskType = questTemplate.TaskType,
            Required = questTemplate.Required,
            RewardType = questTemplate.RewardType,
            RewardAmount = questTemplate.RewardAmount,
            ChannelId = questTemplate.ChannelId,
            Weight = questTemplate.Weight,
        };
        try
        {
            await _questsService.AddQuest(quest);
            return Ok(new { Success = true, Status = StatusCodes.Status200OK });
        }
        catch (QuestTemplateAlreadyExistsException ex)
        {
            return BadRequest(new { ex.Code, ex.Type, ex.Task, ex.Required });
        }
    }
    [HttpDelete("templates")]
    public async Task<IActionResult> RemoveQuest([FromBody] QuestTemplateDTO questTemplate)
    {
        var quest = new Quest
        {
            GuildId = questTemplate.GuildId,
            Type = questTemplate.Type,
            TaskType = questTemplate.TaskType,
            Required = questTemplate.Required,
            RewardType = questTemplate.RewardType,
            RewardAmount = questTemplate.RewardAmount,
            ChannelId = questTemplate.ChannelId,
            Weight = questTemplate.Weight,
        };
        try
        {
            await _questsService.RemoveQuest(quest);
            return Ok(new { Success = true, Status = StatusCodes.Status200OK });
        }
        catch (QuestTemplateDoesNotExistException ex)
        {
            return BadRequest(new { ex.Code, ex.Type, ex.Task, ex.Required });
        }
    }
    [HttpGet]
    public async Task<IActionResult> GetAllQuests(long guildId)
    {
        return Ok(await _questsService.GetAllQuests(guildId));
    }
    [HttpGet("board-img")]
    public async Task<IActionResult> GetQuestBoardImg(long guildId)
    {
        var dailyQuests = await _questsService.GetAvailableNowGuildQuests(guildId, QuestTypes.Daily);
        var weeklyQuests = await _questsService.GetAvailableNowGuildQuests(guildId, QuestTypes.Weekly);
        var eventQuests = await _questsService.GetAvailableNowGuildQuests(guildId, QuestTypes.Event);

        // var img = await MakeImg(dailyQuests, weeklyQuests, eventQuests);
        return Ok(new { Img = "https://i.imgur.com/zX5GJb3.png" });
    }
    [HttpGet("available/{userId}/{type}")]
    public async Task<IActionResult> GetAvailableNowGuildQuests(long guildId, long userId, QuestTypes type)
    {
        var quests = await _questsService.GetAvailableNowGuildQuests(guildId, type);
        var result = new List<QuestDTO>();

        foreach (var uq in quests)
        {
            var isAccepted = await _questsService.IsAccepted(userId, uq.Id);
            result.Add(new QuestDTO
            {
                Id = uq.Id,
                GuildId = uq.GuildId,
                Guild = uq.Guild,
                Type = uq.Type,
                TaskType = uq.TaskType,
                ChannelId = uq.ChannelId,
                Required = uq.Required,
                RewardType = uq.RewardType,
                RewardAmount = uq.RewardAmount,
                CompletableUntil = uq.CompletableUntil,
                AcceptedByUser = isAccepted
            });
        }

        return Ok(result);
    }
    [HttpPost("accept/{userId}/{questId}")]
    public async Task<IActionResult> AcceptQuest(long userId, int questId)
    {
        try
        {
            await _questsService.AcceptQuest(userId, questId);
            return Ok(new { Success = true, Status = StatusCodes.Status200OK });
        }
        catch (Exception)
        {
            return BadRequest(new { Code = "00000" });
        }
    }
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserQuests(long guildId, long userId)
    {
        var userQuests = await _questsService.GetUserQuests(guildId, userId);
        return Ok(userQuests.Select(uq => new QuestDTO
        {
            Id = uq.QuestId,
            GuildId = uq.Quest.GuildId,
            Guild = uq.Quest.Guild,
            UserId = uq.UserId,
            User = uq.User,
            Type = uq.Quest.Type,
            TaskType = uq.Quest.TaskType,
            ChannelId = uq.Quest.ChannelId,
            Required = uq.Quest.Required,
            Progress = uq.Progress,
            RewardType = uq.Quest.RewardType,
            RewardAmount = uq.Quest.RewardAmount,
            AssignedAt = uq.AssignedAt,
            CompletedAt = uq.CompletedAt,
            CompletableUntil = uq.Quest.CompletableUntil,
            IsCompleted = uq.IsCompleted,
            AcceptedByUser = true
        }));
    }
    [HttpPost]
    [Route("/api/v{version:apiVersion}/[controller]/update")]
    public async Task<IActionResult> UpdateQuests()
    {
        await _questsService.UpdateQuests();
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
}