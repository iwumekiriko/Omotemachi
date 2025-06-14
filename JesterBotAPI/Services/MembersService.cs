using DellArteAPI.DTOS.V1.Jester;
using DellArteAPI.Exceptions.Jester.Members;
using DellArteAPI.Models.V1;
using DellArteAPI.Models.V1.Jester.Config;
using DellArteAPI.Models.V1.Jester.Settings;
using DellArteAPI.Models.V1.Jester.Statistics;
using DellArteAPI.Models.V1.Jester.Top;
using DellArteAPI.Services.Jester;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DellArteAPI.Services;

public interface IMembersService
{
    Member GetMember(long guildId, long userId);
    Task<Member> GetMemberAsync(long guildId, long userId);
    Task<MemberDTO> GetMemberDTOAsync(long guildId, long userId);
    Guild GetGuild(long guildId);
    Task<Guild> GetGuildAsync(long guildId);
    User GetUser(long userId);
    Task<User> GetUserAsync(long userId);
    Task<Member> UpdateCoinsAsync(long guildId, long userId, int amount);
    Task<Member> UpdateCrystalsAsync(long guildId, long userId, int amount);
    Task SetExpMultiplier(long guildId, long userId, int value);
    Task<Member> HandleMessage(long guildId, long userId);
    Task<(Member, int)> HandleVoice(long guildId, long userId, int seconds, bool muted);
    Task OnMemberJoin(long guildId, long userId);
    Task OnMemberLeave(long guildId, long userId);
    Task MemberUpdate(Member member);
    Task<TopDTO> GetTop(long guildId, long userId, TopTypes type);
}

public class MembersService(
    AppContext context,
    ILogger<MembersService> logger,
    IConfigService<ExperienceConfig> expConfigService,
    Dictionary<string, int> voiceSessions,
    IStatisticsService statistics,
    ITopService topService,
    IUserSettingsService userSettings
) : ServiceBase(context, logger), IMembersService
{
    private readonly IConfigService<ExperienceConfig> _expConfigService = expConfigService;
    private readonly Dictionary<string, int> _voiceSessions = voiceSessions;
    private readonly IStatisticsService _statistics = statistics;
    private readonly ITopService _topService = topService;
    private readonly IUserSettingsService _userSettings = userSettings;

    public Member GetMember(long guildId, long userId)
    {
        var member = _context.Members
            .Where(m => m.UserId == userId
                && m.GuildId == guildId)
            .FirstOrDefault();

        if (member == null)
        {
            member = new Member
            {
                UserId = userId,
                GuildId = guildId,
            };
            _context.Members.Add(member);
            _context.SaveChanges();
        }
        return member;
    }
    public async Task<Member> GetMemberAsync(long guildId, long userId)
    {
        var member = await _context.Members
            .Where(m => m.UserId == userId
                && m.GuildId == guildId)
            .FirstOrDefaultAsync();

        if (member == null)
        {
            var guild = await _context.Guilds.FirstOrDefaultAsync(g => g.Id == guildId) ?? new Guild { Id = guildId };
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? new User { Id = userId };

            member = new Member
            {
                User = user,
                Guild = guild,
            };
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
        }
        return member;
    }
    public async Task<MemberDTO> GetMemberDTOAsync(long guildId, long userId)
    {
        _logger.LogInformation("123123");

        var member = await GetMemberAsync(guildId, userId);
        var messageStatistics = await _statistics.GetStatistics<MessagesStatistics>(guildId, userId);
        var voiceStatistics = await _statistics.GetStatistics<VoiceStatistics>(guildId, userId);

        return new MemberDTO
        {
            GuildId = member.GuildId,
            Guild = member.Guild,
            UserId = member.UserId,
            User = member.User,
            Active = member.Active,
            Experience = member.Experience,
            ExpMultiplier = member.ExpMultiplier,
            Coins = member.Coins,

            Crystals = member.Crystals,
            MessageCount = messageStatistics.MessagesWritenCount,
            VoiceTime = voiceStatistics.VoiceTimeMuted + voiceStatistics.VoiceTimeUnMuted,
            JoinedAt = member.JoinedAt,
            IsBot = member.IsBot,
        };
    }
    public Guild GetGuild(long guildId)
    {
        var guild = _context.Guilds.FirstOrDefault(g => g.Id == guildId);
        if (guild == null)
        {
            guild = new Guild { Id = guildId };
            _context.Guilds.Add(guild);
            _context.SaveChanges();
        }
        return guild;
    }
    public async Task<Guild> GetGuildAsync(long guildId)
    {
        var guild = await _context.Guilds.FirstOrDefaultAsync(g => g.Id == guildId);
        if (guild == null)
        {
            guild = new Guild { Id = guildId };
            _context.Guilds.Add(guild);
            await _context.SaveChangesAsync();
        }
        return guild;
    }
    public User GetUser(long userId)
    {
        var user = _context.Users.FirstOrDefault(g => g.Id == userId);
        if (user == null)
        {
            user = new User { Id = userId };
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        return user;
    }
    public async Task<User> GetUserAsync(long userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(g => g.Id == userId);
        if (user == null)
        {
            user = new User { Id = userId };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        return user;
    }
    public async Task<Member> UpdateCoinsAsync(long guildId, long userId, int amount)
    {
        var member = await GetMemberAsync(guildId, userId);
        if (member.IsBot)
            return member;

        member.Coins += amount;
        if (member.Coins < 0)
        {
            var originalAmount = member.Coins - amount;
            throw new NotEnoughCoinsException(
                current: originalAmount,
                needed: Math.Abs(amount)
            );
        }
        await _statistics.IncrementStatistics<MembersStatistics>(
            guildId, userId, s => s.CoinsAmountChangedCount);

        await _context.SaveChangesAsync();
        return member;
    }
    public async Task<Member> UpdateCrystalsAsync(long guildId, long userId, int amount)
    {
        var member = await GetMemberAsync(guildId, userId);
        if (member.IsBot)
            return member;

        member.Crystals += amount;
        if (member.Crystals < 0)
        {
            var originalAmount = member.Crystals - amount;
            throw new NotEnoughCrystalsException(
                current: originalAmount,
                needed: Math.Abs(amount)
            );
        }
        await _statistics.IncrementStatistics<MembersStatistics>(
            guildId, userId, s => s.CrystalsAmountChangedCount);

        await _context.SaveChangesAsync();
        return member;
    }
    public async Task SetExpMultiplier(long guildId, long userId, int value)
    {
        var member = await GetMemberAsync(guildId, userId);
        member.ExpMultiplier = value;
        await _context.SaveChangesAsync();
    }
    public async Task<Member> HandleMessage(long guildId, long userId)
    {
        var member = await GetMemberAsync(guildId, userId);

        await AddMessageExperience(member);
        return member;
    }
    public async Task<(Member, int)> HandleVoice(long guildId, long userId, int seconds, bool muted)
    {
        var member = await GetMemberAsync(guildId, userId);

        var minutes = await AddVoiceExperience(member, seconds, muted);
        return (member, minutes);
    }
    private async Task AddMessageExperience(Member member)
    {
        var uSetting = await _userSettings.GetUserSetting(
            member.GuildId, member.UserId, SettingTypes.ExpDisabling);
        if (uSetting != null && !uSetting.State)
        {
            var expConfig = await _expConfigService.GetOrCreateConfigAsync(member.GuildId);
            var expToAdd = (expConfig.ExpForMessage ?? 3) * member.ExpMultiplier;
            member.Experience += expToAdd;

            if (member.ExpMultiplier != 1)
                await _statistics.IncrementStatistics<InventoryStatistics>(
                    member.GuildId, member.UserId, s => s.ExpGainedWithBoosters, expToAdd);
        }

        await _statistics.IncrementStatistics<MessagesStatistics>(
            member.GuildId, member.UserId, s => s.MessagesWritenCount);

        await _context.SaveChangesAsync();
    }
    private async Task<int> AddVoiceExperience(Member member, int seconds, bool muted)
    {
        var sessionKey = $"{member.GuildId}-{member.UserId}-{muted}";

        if (_voiceSessions.TryGetValue(sessionKey, out var vcSeconds))
            vcSeconds += seconds;
        else
        {
            vcSeconds = seconds;
            _voiceSessions[sessionKey] = vcSeconds;
        }

        int totalMinutes = vcSeconds / 60;
        if (totalMinutes > 0)
        {
            var uSetting = await _userSettings.GetUserSetting(
                member.GuildId, member.UserId, SettingTypes.ExpDisabling);
            if (uSetting != null && !uSetting.State)
            {
                var expConfig = await _expConfigService.GetOrCreateConfigAsync(member.GuildId);
                int expToAdd = (expConfig.ExpForVoiceMinute ?? 1) * member.ExpMultiplier * totalMinutes;
                member.Experience += expToAdd;

                if (member.ExpMultiplier != 1)
                    await _statistics.IncrementStatistics<InventoryStatistics>(
                        member.GuildId, member.UserId, s => s.ExpGainedWithBoosters, expToAdd);
            }

            vcSeconds %= 60;
        }
        _voiceSessions[sessionKey] = vcSeconds;
        Expression<Func<VoiceStatistics, int>> propertySelector = muted ?
            s => s.VoiceTimeMuted : s => s.VoiceTimeUnMuted;

        await _statistics.IncrementStatistics<VoiceStatistics>(
            member.GuildId, member.UserId, propertySelector, seconds);

        await _context.SaveChangesAsync();
        return totalMinutes;
    }
    public async Task OnMemberJoin(long guildId, long userId)
    {
        var member = await GetMemberAsync(guildId, userId);
        member.Active = true;
        await _context.SaveChangesAsync();
    }
    public async Task OnMemberLeave(long guildId, long userId)
    {
        var member = await GetMemberAsync(guildId, userId);
        member.Active = false;
        await _context.SaveChangesAsync();
    }
    public async Task MemberUpdate(Member member)
    {
        var memberData = await GetMemberAsync(
            member.GuildId, member.UserId);
        memberData.Experience = member.Experience;
        memberData.Coins = member.Coins;
        memberData.Crystals = member.Crystals;
        memberData.ExpMultiplier = member.ExpMultiplier;
        await _context.SaveChangesAsync();
    }
    public async Task<TopDTO> GetTop(long guildId, long userId, TopTypes type)
    {
        return type switch
        {   
            TopTypes.Messages => await _topService.GetGenericTopFromStatisticsAsync(
                _context.MessagesStatistics,
                ms => ms.MessagesWritenCount,
                guildId, userId),   
            TopTypes.Voice => await _topService.GetGenericTopFromStatisticsAsync(
                _context.VoiceStatistics,
                vs => vs.VoiceTimeMuted + vs.VoiceTimeUnMuted,
                guildId, userId),
            TopTypes.Experience => await _topService.GetExperienceTopAsync(guildId, userId),
            TopTypes.Currency => await _topService.GetCurrencyTopAsync(guildId, userId),
            TopTypes.Lootboxes => await _topService.GetGenericTopFromStatisticsAsync(
                _context.LootboxesStatistics,
                ls => 
                    ls.RolesLootboxesOpenedCount +
                    ls.BackgroundsLootboxesOpenedCount,
                guildId, userId),
            TopTypes.Quests => await _topService.GetGenericTopFromStatisticsAsync(
                _context.QuestsStatistics,
                qs => qs.QuestsCompletedCount,
                guildId, userId),
            TopTypes.DND => await _topService.GetGenericTopFromStatisticsAsync(
                _context.DNDStatistics,
                ds => ds.DNDDiceRolledMaxCount - ds.DNDDiceRolledMinCount,
                guildId, userId),
            TopTypes.Duets => await _topService.GetDuetsTopAsync(guildId, userId),
            _ => throw new ArgumentException("Unknown Top type")
        };
    }
}