using Omotemachi.Models.V1;
using Microsoft.EntityFrameworkCore;
using Omotemachi.Models.V1.Domain.Jester.Config;
using Omotemachi.Infrastructure.Persistance.AppContext;

namespace Omotemachi.Services.Jester;

public interface IConfigService<T> where T : class, IConfig
{
    Task<T> GetOrCreateConfigAsync(long guildId);
    Task UpdateConfigAsync(T config);
}
public class ConfigService<T>(
    AppDbContext context,
    ILogger<ConfigService<T>> logger
) : ServiceBase(context, logger), IConfigService<T> where T : class, IConfig
{
    public async Task<T> GetOrCreateConfigAsync(long guildId)
    {
        var config = await _context.Set<T>().FirstOrDefaultAsync(c => c.GuildId == guildId);
        config ??= (T)Activator.CreateInstance(typeof(T), guildId)!;
        return config;
    }

    public async Task UpdateConfigAsync(T config)
    {
        var dbConfig = await _context.Set<T>()
            .FirstOrDefaultAsync(c => c.GuildId == config.GuildId);

        if (dbConfig == null)
        {
            var existingGuild = await _context.Guilds.FindAsync(config.GuildId);
            if (existingGuild != null)
            {
                _context.Attach(existingGuild);
                config.Guild = existingGuild;
            }
            _context.Set<T>().Add(config);
        }
        else
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (property.GetValue(dbConfig) == property.GetValue(config))
                    continue;

                var newValue = property.GetValue(config);
                if (newValue != null)
                {
                    property.SetValue(dbConfig, newValue);
                }
            }
        }
        await _context.SaveChangesAsync();
    }
}