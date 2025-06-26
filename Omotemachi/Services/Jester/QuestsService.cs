using Omotemachi.Exceptions.Jester.Quests;
using Omotemachi.Models.V1.Jester.Items;
using Omotemachi.Models.V1.Jester.Lootboxes;
using Omotemachi.Models.V1.Jester.Quests;
using Omotemachi.Models.V1.Jester.Settings;
using Omotemachi.Models.V1.Statistics;
using Omotemachi.Services.Wacky;
using Omotemachi.Tools;
using Microsoft.EntityFrameworkCore;

namespace Omotemachi.Services.Jester;

public interface IQuestsService
{
    Task AddQuest(Quest quest);
    Task RemoveQuest(Quest quest);
    Task<List<Quest>> GetAllQuests(long guildId);
    Task<List<Quest>> GetAvailableNowGuildQuests(long guildId, QuestTypes type);
    Task AcceptQuest(long userId, int questId);
    Task<bool> IsAccepted(long userId, int questId);
    Task<List<UserQuest>> GetUserQuests(long guildId, long userId);
    Task AddProgress(long guildId, long userId, long channelId, int progressCount);
    Task UpdateQuests();
}

public class QuestsService(
    AppContext context,
    ILogger<QuestsService> logger,
    IMembersService membersService,
    IInventoryService inventoryService,
    IConfiguration config,
    IStatisticsService statistics,
    IUserSettingsService uSettingsService,
    IPacksService packsService
) : ServiceBase(context, logger), IQuestsService
{
    private readonly IConfiguration _config = config;
    private readonly IMembersService _membersService = membersService;
    private readonly IInventoryService _inventoryService = inventoryService;
    private readonly IStatisticsService _statistics = statistics;
    private readonly IUserSettingsService _uSettingsService = uSettingsService;
    private readonly IPacksService _packsService = packsService;
    private readonly Random _random = new();

    public async Task AddQuest(Quest quest)
    {
        var existingQuest = _context.Quests
            .FirstOrDefault(q => 
                q.GuildId == quest.GuildId &&
                q.Type == quest.Type &&
                q.TaskType == quest.TaskType &&
                q.Required == quest.Required &&
                q.ChannelId == quest.ChannelId);

        if (existingQuest != null)
            throw new QuestTemplateAlreadyExistsException(
                type: quest.Type,
                task: quest.TaskType,
                required: quest.Required,
                channelId: quest.ChannelId
            );

        _context.Quests.Add(quest);
        await _context.SaveChangesAsync();
    }
    public async Task RemoveQuest(Quest quest)
    {
        var existingQuest = _context.Quests
            .FirstOrDefault(q =>
                q.GuildId == quest.GuildId &&
                q.Type == quest.Type &&
                q.TaskType == quest.TaskType &&
                q.Required == quest.Required &&
                q.ChannelId == quest.ChannelId);

        if (existingQuest == null)
            throw new QuestTemplateDoesNotExistException(
                type: quest.Type,
                task: quest.TaskType,
                required: quest.Required,
                channelId: quest.ChannelId
            );

        existingQuest.IsValid = false;
        await _context.SaveChangesAsync();
    }
    public async Task<List<Quest>> GetAllQuests(long guildId)
    {
        return await _context.Quests
            .Where(q => q.GuildId == guildId)
            .ToListAsync();
    }
    public async Task<List<Quest>> GetAvailableNowGuildQuests(long guildId, QuestTypes type)
    {
        var availableQuests = await _context.Quests
            .Where(q => q.GuildId == guildId && q.Type == type && q.IsAvailableNow)
            .ToListAsync();

        if (availableQuests.Count == 0)
        {
            availableQuests = await GenerateRandomQuests(guildId, type);
            await _context.SaveChangesAsync();
        }
        return availableQuests;
    }
    public async Task AcceptQuest(long userId, int questId)
    {
        var accepted = _context.UserQuests.FirstOrDefault(q => 
            q.UserId == userId && q.QuestId == questId);

        if (accepted != null)
            throw new ArgumentException("ALREADY ACCEPTED");

        var userQuest = new UserQuest
        {
            QuestId = questId,
            UserId = userId,
        };
        var quest = _context.UserQuests.Add(userQuest);
        await _statistics.IncrementStatistics<QuestsStatistics>(
            _context.Quests.Find(questId)!.GuildId, userId,
            s => s.QuestsAssignedCount);

        await _context.SaveChangesAsync();
    }
    public async Task<bool> IsAccepted(long userId, int questId) => await _context.UserQuests
            .FirstOrDefaultAsync(uq => uq.UserId == userId && uq.QuestId == questId) != null;
    public async Task<List<UserQuest>> GetUserQuests(
        long guildId,
        long userId
    )
    {
        return await _context.UserQuests
            .Include(uq => uq.Quest)
            .Where(uq => uq.Quest.GuildId == guildId && uq.UserId == userId)
            .ToListAsync();
    }
    public async Task AddProgress(long guildId, long userId, long channelId, int progressCount)
    {
        var userQuests = await _context.UserQuests
            .Include(uq => uq.Quest)
            .Where(uq =>
                uq.UserId == userId &&
                uq.Quest.GuildId == guildId &&
                uq.Quest.ChannelId == channelId &&
                uq.CompletedAt == null)
            .ToListAsync();

        foreach(var userQuest in userQuests)
        {
            userQuest.Progress += progressCount;
            if (userQuest.Progress >= userQuest.Quest.Required)
            {
                userQuest.CompletedAt = TimeConverter.GetCurrentTime(); ;
                await _statistics.IncrementStatistics<QuestsStatistics>(
                    guildId, userId,
                    s => s.QuestsCompletedCount);

                await HandleQuestRewards(userQuest);
            }
        }
        await _context.SaveChangesAsync();
    }
    public async Task UpdateQuests()
    {
        await RemoveInvalidQuests();
        await MakeExpiredQuestsUnavailable();
        await RemoveExpiredUserQuests();
        await AutoAcceptQuests();
        //await MakeCompleted();
        await _context.SaveChangesAsync();
    }
    private async Task AutoAcceptQuests()
    {
        var guilds = await _context.Guilds.AsNoTracking().ToListAsync();
        foreach (var guild in guilds)
        {
            var settingId = await _uSettingsService.GetGuildSettingId(
                guild.Id, SettingTypes.AutoQuestsTake);

            var userIds = await _context.UserSettings
                .Where(us => us.State && us.SettingId == settingId)
                .Select(us => us.UserId)
                .Distinct()
                .ToListAsync();

            if (userIds.Count == 0)
                continue;

            var avDailyQuests = await GetAvailableNowGuildQuests(guild.Id, QuestTypes.Daily);
            var avWeeklyQuests = await GetAvailableNowGuildQuests(guild.Id, QuestTypes.Weekly);
            var avEventQuests = await GetAvailableNowGuildQuests(guild.Id, QuestTypes.Event);

            var allQuests = avDailyQuests
                .Concat(avWeeklyQuests)
                .Concat(avEventQuests)
                .ToList();

            if (allQuests.Count == 0)
                continue;

            foreach (var quest in allQuests)
            {
                foreach (var userId in userIds)
                    try
                    {
                        await AcceptQuest(userId, quest.Id);
                    }
                    catch
                    {
                        continue;
                    }
            }
        }
    }
    private async Task RemoveInvalidQuests()
    {
        var invalidQuests = await _context.Quests
            .Where(q => q.IsValid == false && DateTime.Now > q.CompletableUntil)
            .ToListAsync();

        _context.Quests.RemoveRange(invalidQuests);
    }
    private async Task MakeExpiredQuestsUnavailable()
    {
        await _context.Quests
            .Where(q => q.IsAvailableNow && DateTime.Now > q.CompletableUntil)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(q => q.IsAvailableNow, false));
    }
    private async Task RemoveExpiredUserQuests()
    {
        var expiredQuests = await _context.UserQuests
            .Where(uq => DateTime.Now > uq.Quest.CompletableUntil)
            .ToListAsync();

        _context.UserQuests.RemoveRange(expiredQuests);
    }
    private async Task MakeCompleted()
    {
        await _context.UserQuests
            .Where(uq => DateTime.Now > uq.CompletedAt && uq.IsCompleted != true && uq.Progress >= uq.Quest.Required)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(uq => uq.IsCompleted, true));
    }
    private async Task<List<Quest>> GenerateRandomQuests(
        long guildId,
        QuestTypes type
    )
    {
        var quests = type switch
        {
            QuestTypes.Daily => await GenerateRandomDailyQuests(guildId, int.Parse(_config["Quests:DailyQuestsCount"]!)),
            QuestTypes.Weekly => await GenerateRandomWeeklyQuests(guildId, int.Parse(_config["Quests:WeeklyQuestsCount"]!)),
            _ => []
        };
        await MakeAvailableQuests(quests);
        return quests;
    }
    private async Task<List<Quest>> GenerateRandomWeeklyQuests(long guildId, int count)
    {
        if (count == 0)
            return [];

        var allWeeklyGuildQuests = await _context.Quests
            .Where(q =>
                q.Type == QuestTypes.Weekly &&
                q.GuildId == guildId &&
                q.IsAvailableNow == false)
            .ToListAsync();

        if (allWeeklyGuildQuests.Count == 0)
            return [];

        if (allWeeklyGuildQuests.Count <= count)
            return allWeeklyGuildQuests;

        return RandomQuests(allWeeklyGuildQuests, count);
    }
    private async Task<List<Quest>> GenerateRandomDailyQuests(long guildId, int count)
    {
        if (count == 0)
            return [];

        var allDailyGuildQuests = await _context.Quests
            .Where(q =>
                q.Type == QuestTypes.Daily &&
                q.GuildId == guildId &&
                q.IsAvailableNow == false)
            .ToListAsync();

        if (allDailyGuildQuests.Count == 0)
            return [];

        if (allDailyGuildQuests.Count <= count)
            return allDailyGuildQuests;

        return RandomQuests(allDailyGuildQuests, count);
    }
    private async Task MakeAvailableQuests(List<Quest> quests)
    {
        TimeZoneInfo moscowTimeZone;
        moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");

        DateTimeOffset utcNow = DateTimeOffset.UtcNow;
        DateTimeOffset moscowTime = TimeZoneInfo.ConvertTime(utcNow, moscowTimeZone);
        DateTime todayInMoscow = moscowTime.Date;

        foreach (var quest in quests)
        {
            int daysToComplete = quest.Type switch
            {
                QuestTypes.Daily => 1,
                QuestTypes.Weekly => (7 - (int)todayInMoscow.DayOfWeek + (int)DayOfWeek.Monday) % 7,
                _ => -1
            };

            if (daysToComplete < 0) continue;
            DateTime completionDate = todayInMoscow.AddDays(daysToComplete == 0 ? 7 : daysToComplete);
            TimeSpan offset = moscowTimeZone.GetUtcOffset(completionDate);

            quest.CompletableUntil = new DateTimeOffset(completionDate, offset);
            quest.IsAvailableNow = true;
        }

        await _context.SaveChangesAsync();
    }
    private List<Quest> RandomQuests(List<Quest> quests, int count)
    {
        var typeGroups = quests
            .GroupBy(q => q.TaskType)
            .ToDictionary(
                g => g.Key,
                g => new TypeGroup
                {
                    Quests = [.. g],
                    Multiplier = 1.0f
                });

        var result = new List<Quest>();

        for (int i = 0; i < count; i++)
        {
            var availableTypes = typeGroups
                .Where(kvp => kvp.Value.Quests.Count > 0 && kvp.Value.Multiplier > 0)
                .ToList();

            if (availableTypes.Count == 0)
                break;

            float totalWeight = 0;
            var typeWeightList = new List<(QuestTaskTypes Type, float CumulativeWeight)>();
            foreach (var kpv in availableTypes)
            {
                var group = kpv.Value;
                float sumWeight = group.Quests.Sum(q => q.Weight);
                float typeWeight = sumWeight * group.Multiplier;
                totalWeight += typeWeight;
                typeWeightList.Add((kpv.Key, totalWeight));
            }

            if (totalWeight <= 0)
                break;

            float randomValue = (float)_random.NextDouble() * totalWeight;
            QuestTaskTypes selectedType = typeWeightList.First().Type;
            foreach (var (type, weight) in typeWeightList)
            {
                if (randomValue <= weight)
                {
                    selectedType = type;
                    break;
                }   
            }

            var selectedGroup = typeGroups[selectedType];
            var questsInType = selectedGroup.Quests;
            float sumTypeWeights = questsInType.Sum(q => q.Weight);
            float questRandom = (float)_random.NextDouble() * sumTypeWeights;

            Quest? selectedQuest = null;
            float currentWeigth = 0;
            foreach (var quest in questsInType)
            {
                currentWeigth += quest.Weight;
                if (currentWeigth >= questRandom)
                {
                    selectedQuest = quest;
                    break;
                }
            }

            selectedQuest ??= questsInType.Last();
            result.Add(selectedQuest);
            questsInType.Remove(selectedQuest);
            selectedGroup.Multiplier = Math.Max(selectedGroup.Multiplier * 0.5f, 0.001f);
        }
        return result;
    }
    private async Task HandleQuestRewards(UserQuest userQuest)
    {
        var userId = userQuest.UserId;
        var guildId = userQuest.Quest.GuildId;
        var rewardType = userQuest.Quest.RewardType;
        var rewardAmount = userQuest.Quest.RewardAmount;

        switch (rewardType)
        {
            case QuestRewardTypes.Coins:
                await _membersService.UpdateCoinsAsync(guildId, userId, rewardAmount);
                await _statistics.IncrementStatistics<QuestsStatistics>(
                    guildId, userId, s => s.CoinsFromQuestsCount, rewardAmount);
                break;

            case QuestRewardTypes.Crystals:
                await _membersService.UpdateCrystalsAsync(guildId, userId, rewardAmount);
                await _statistics.IncrementStatistics<QuestsStatistics>(
                    guildId, userId, s => s.CrystallsFromQuestsCount, rewardAmount);
                break;

            case QuestRewardTypes.LootboxKey:
                var lootboxKey = new LootboxKey {
                    GuildId = guildId,
                    Type = LootboxTypes.RolesLootbox //TODO: Make conf file with active lootboxes
                };
                await _inventoryService.UpdateInventoryAsync(guildId, userId, lootboxKey, rewardAmount);
                await _statistics.IncrementStatistics<QuestsStatistics>(
                    guildId, userId, s => s.LootboxKeysFromQuestsCount, rewardAmount);
                break;
            case QuestRewardTypes.CardsPack:
                await _packsService.UpdateUserPackAmount(
                    guildId,
                    userId,
                    await _packsService.GetGeneralPackId() ?? 1,
                    rewardAmount
                );
                await _statistics.IncrementStatistics<QuestsStatistics>(
                    guildId, userId, s => s.CardsPacksFromQuestsCount, rewardAmount);
                break;
        }
    }
    private class TypeGroup
    {
        public List<Quest> Quests { get; set; }
        public float Multiplier { get; set; }
    }
}
