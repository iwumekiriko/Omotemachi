using DellArteAPI.Exceptions.Jester.Interactions;
using DellArteAPI.Models.V1.Jester.Interactions;
using DellArteAPI.Models.V1.Jester.Settings;
using Microsoft.EntityFrameworkCore;

namespace DellArteAPI.Services.Jester;

public interface IInteractionsService
{
    Task<InteractionsAsset> GetAssetAsync(long guildId, InteractionsActions action, InteractionsTypes type);
    Task UploadGifsAsync(long guildId, List<InteractionsAsset> assets);
    Task<bool> IsInteractionRestricted(long guildId, long targetId);
}

public class InteractionsService(
    AppContext context,
    ILogger<InteractionsService> logger,
    IUserSettingsService uSettingsService
) : ServiceBase(context, logger), IInteractionsService
{
    private readonly IUserSettingsService _uSettingsService = uSettingsService;
    private readonly Random _random = new();

    public async Task<bool> IsInteractionRestricted(long guildId, long targetId)
    {
        var targetSettings = await _uSettingsService.GetUserSetting(guildId, targetId, SettingTypes.RestrictInteractions);
        return targetSettings?.State == true;
    }
    public async Task<InteractionsAsset> GetAssetAsync(long guildId, InteractionsActions action, InteractionsTypes type)
    {
        var assets = await _context.InteractionsAssets
            .Where(ia => ia.Action == action && ia.Type == type)
            .ToListAsync();

        if (assets.Count == 0)
            throw new NoAvailableGifsException(action, type);

        return assets[_random.Next(assets.Count)];
    }

    public async Task UploadGifsAsync(long guildId, List<InteractionsAsset> assets)
    {
        var existingUrls = new HashSet<string>(
        await _context.InteractionsAssets
            .Select(a => a.AssetUrl)
            .ToListAsync());

        var assetsToAdd = assets
            .Where(a => !existingUrls.Contains(a.AssetUrl))
            .ToList();

        if (assetsToAdd.Count != 0)
        {
            await _context.InteractionsAssets.AddRangeAsync(assetsToAdd);
            await _context.SaveChangesAsync();
        }
    }
}
