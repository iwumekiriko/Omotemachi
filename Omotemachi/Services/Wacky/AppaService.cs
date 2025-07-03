using Microsoft.EntityFrameworkCore;
using Omotemachi.DTOS.V1.Wacky;
using Omotemachi.Exceptions.Wacky;
using Omotemachi.Models.V1.Wacky.Appa;
using Omotemachi.Tools;

namespace Omotemachi.Services.Wacky;

public interface IAppaService
{
    Task<Appa?> GetAppaByIdAsync(int id);
    Task<Appa?> GetAppaByNameAsync(string name);
    Task AddAppaAsync(Appa appa);
    Task<Appa?> GetRandomAppaAsync();
    Task<AppaDTO?> GetAndCountRandomUserAppaAsync(long guildId, long userId);
    Task<List<AppaDTO>> GetUserAppasAsync(long guildId, long userId);
}

public class AppaService(
    AppContext context,
    ILogger<AppaService> logger,
    IConfiguration config
) : ServiceBase(context, logger), IAppaService
{
    private readonly IConfiguration _config = config;

    public async Task AddAppaAsync(Appa appa)
    {
        _context.Appas.Add(appa);
        await _context.SaveChangesAsync();
    }
    public async Task<List<Appa>> GetAppasAsync()
    {
        return await _context.Appas.ToListAsync();
    }
    public async Task<Appa?> GetAppaByIdAsync(int id)
    {
        return await _context.Appas.FirstOrDefaultAsync(a => a.Id == id);
    }
    public async Task<Appa?> GetAppaByNameAsync(string name)
    {
        return await _context.Appas.FirstOrDefaultAsync(a => a.Name == name);
    }
    public async Task<Appa?> GetRandomAppaAsync()
    {
        return await _context.Appas
            .OrderBy(x => Guid.NewGuid())
            .FirstAsync();
    }
    public async Task<AppaDTO?> GetAndCountRandomUserAppaAsync(long guildId, long userId)
    {
        var timeoutData = await _context.TimeoutAppaCatches
             .Where(t => t.GuildId == guildId && t.UserId == userId)
             .FirstOrDefaultAsync();

        if (timeoutData != null && timeoutData.LastCatch >= TimeConverter.Today)
            throw new CommandTimeoutException(
                timeLeft: TimeConverter.Tomorrow - TimeConverter.GetCurrentTime());

        var appa = await GetRandomAppaAsync();
        if (appa == null)
            return null;

        var uAppa = await _context.UserAppas
            .Where(ua => ua.GuildId == guildId && ua.UserId == userId)
            .FirstOrDefaultAsync();

        if (uAppa == null)
        {
            uAppa = new UserAppa
            {
                GuildId = guildId,
                UserId = userId,
                Appa = appa,
            };
            _context.UserAppas.Add(uAppa);
        }
        uAppa.Amount++;

        if (timeoutData == null)
        {
            timeoutData = new TimeoutAppaCatch
            {
                GuildId = guildId,
                UserId = userId
            };
            _context.TimeoutAppaCatches.Add(timeoutData);
        }
        else
        {
            timeoutData.LastCatch = TimeConverter.GetCurrentTime();
        }

        await _context.SaveChangesAsync();
        return MapToDTO(uAppa);
    }
    public async Task<List<AppaDTO>> GetUserAppasAsync(long guildId, long userId)
    {
        var uAppas = await _context.UserAppas
            .Where(ua => 
                ua.GuildId == guildId &&
                ua.UserId == userId &&
                ua.Amount > 0)
            .ToListAsync();

        var appas = await GetAppasAsync();
        var missingAppas = appas
            .Where(a => !uAppas.Any(ua => ua.AppaId == a.Id))
            .Select(a => new UserAppa
            {
                GuildId = guildId,
                UserId = userId,
                AppaId = a.Id,
                Appa = new Appa
                {
                    Id = a.Id,
                    Name = "Unknown",
                    AssetUrl = _config["Appas:LockedAssetUrl"]!
                }
            })
            .ToList();

        return [.. uAppas.Concat(missingAppas).OrderBy(ua => ua.AppaId).Select(ua => MapToDTO(ua))];
    }
    private static AppaDTO MapToDTO(UserAppa uAppa)
    {
        return new AppaDTO
        {
            GuildId = uAppa.GuildId,
            UserId = uAppa.UserId,
            Id = uAppa.Appa.Id,
            Name = uAppa.Appa.Name,
            AssetUrl = uAppa.Appa.AssetUrl,
            Amount = uAppa.Amount
        };
    }
}
