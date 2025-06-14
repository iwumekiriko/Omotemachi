using DellArteAPI.DTOS.V1.Jester;
using DellArteAPI.Exceptions.Jester.Settings;
using DellArteAPI.Models.V1.Jester.Settings;
using Microsoft.EntityFrameworkCore;

namespace DellArteAPI.Services.Jester;

public interface IUserSettingsService
{
    Task<int?> GetGuildSettingId(long guildId, SettingTypes type);
    Task<UserSetting?> GetUserSetting(long guildId, long userId, SettingTypes type);
    Task AddGuildSetting(long guildId, SettingTypes type, int? cost);
    Task RemoveGuildSetting(long guildId, SettingTypes type);
    Task<UserSettingsDTO> GetPreparedUserSettingsDTO(long guildId, long userId);
    Task UpdateUserSetting(long guildId, long userId, SettingDTO setting);
}

public class UserSettingsService(
    AppContext context,
    ILogger<UserSettingsService> logger
) : ServiceBase(context, logger), IUserSettingsService
{
    public async Task<int?> GetGuildSettingId(long guildId, SettingTypes type)
    {
        return (await _context.Settings
            .FirstOrDefaultAsync(s => s.Type == type && s.GuildId == guildId))?.Id;
    }
    public async Task<UserSetting?> GetUserSetting(long guildId, long userId, SettingTypes type)
    {
        return await _context.UserSettings
            .Include(us => us.Setting)
            .FirstOrDefaultAsync(us =>
                us.UserId == userId &&
                us.Setting.GuildId == guildId &&
                us.Setting.Type == type
            );
    }
    public async Task AddGuildSetting(long guildId, SettingTypes type, int? cost)
    {
        var existingSetting = await _context.Settings.FirstOrDefaultAsync(s =>
            s.GuildId == guildId && s.Type == type);

        if (existingSetting != null)
            throw new GuildSettingAlreadyExistsException(guildId, type);

        var setting = new Setting { GuildId = guildId, Type = type, Cost = cost ?? 0};
        _context.Settings.Add(setting);
        await _context.SaveChangesAsync();
    }
    public async Task RemoveGuildSetting(long guildId, SettingTypes type)
    {
        var existingSetting = await _context.Settings.FirstOrDefaultAsync(s =>
            s.GuildId == guildId && s.Type == type) ?? throw new GuildSettingDoesNotExistsException(guildId, type);
        _context.Settings.Remove(existingSetting);
        await _context.SaveChangesAsync();
    }
    public async Task<UserSettingsDTO> GetPreparedUserSettingsDTO(long guildId, long userId)
    {
        if ((await _context.Settings.Where(s => s.GuildId == guildId).ToListAsync()).Count == 0)
            throw new NoGuildSettingsAvailableException(guildId);

        var settingsQuery =
           from setting in _context.Settings
           where setting.GuildId == guildId
           join userSetting in _context.UserSettings
               .Where(us => us.UserId == userId)
               on setting.Id equals userSetting.SettingId into userSettingsGroup
           from userSetting in userSettingsGroup.DefaultIfEmpty()
           orderby setting.Cost
           select new SettingDTO
           {
               Id = setting.Id,
               Cost = setting.Cost,
               Bought = userSetting != null || setting.Cost == 0,
               State = userSetting != null && userSetting.State,
               Type = setting.Type
           };

        var settingsDTO = await settingsQuery.ToListAsync();

        return new UserSettingsDTO
        {
            UserId = userId,
            GuildId = guildId,
            Settings = settingsDTO
        };
    }
    public async Task UpdateUserSetting(long guildId, long userId, SettingDTO setting)
    {
        var uSetting = await GetUserSetting(guildId, userId, setting.Type);
        if (uSetting == null)
        {
            uSetting = new UserSetting
            {
                UserId = userId,
                SettingId = setting.Id,
                State = setting.State
            };
            _context.UserSettings.Add(uSetting);
        }
        else
        {
            uSetting.State = setting.State;
        }
        await _context.SaveChangesAsync();
    }
}
