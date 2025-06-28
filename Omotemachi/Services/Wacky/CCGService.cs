using Omotemachi.DTOS.V1.Wacky;
using Omotemachi.Models.V1.Wacky.CCG;
using Omotemachi.Tools;
using Microsoft.EntityFrameworkCore;

namespace Omotemachi.Services.Wacky;

public interface ICCGService
{
    Task<List<Series>> GetSeriesAsync();
    Task<Series?> GetSeriesByIdAsync(int id);
    Task<Series?> GetSeriesByNameAsync(string name);
    Task<Series> CreateSeriesAsync(string name);
    Task<List<Card>> GetAllCardsAsync();
    Task<Card?> GetCardByIdAsync(int id);
    Task<Card> CreateCardAsync(CardDTO card);
    Task<string?> GetRandomCardAssetUrl();
    Task<List<Pack>> GetAllAvailablePacks();
    Task<List<UserCardDTO>> GetUserCollection(long guildId, long userId);
    Task<int> UpdatePacksCount(long guildId, long userId, int packId, int amount);
    Task<List<UserCardDTO>> GetCardsFromPack(long guildId, long userId, int packId, int packAmount);
}
public class CCGService(
    AppContext context,
    ILogger<CCGService> logger,
    IConfiguration config,
    IPacksService packService
) : ServiceBase(context, logger), ICCGService
{
    private readonly IConfiguration _config = config;
    private readonly IPacksService _packService = packService;
    public async Task<List<Series>> GetSeriesAsync()
    {
        return await _context.Series.ToListAsync();
    }
    public async Task<Series?> GetSeriesByIdAsync(int id)
    {
        return await _context.Series.FirstOrDefaultAsync(s => s.Id == id);
    }
    public async Task<Series?> GetSeriesByNameAsync(string name)
    {
        return await _context.Series.FirstOrDefaultAsync(
            s => s.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
    }
    public async Task<Series> CreateSeriesAsync(string name)
    {
        var series = new Series { Name = name };
        _context.Series.Add(series); 
        await _context.SaveChangesAsync();

        return series;
    }
    public async Task<List<Card>> GetAllCardsAsync()
    {
        return await _context.Cards
            .Include(c => c.Series)
            .ToListAsync();
    }
    public async Task<Card?> GetCardByIdAsync(int id)
    {
        return await _context.Cards.FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task<Card> CreateCardAsync(CardDTO cardDTO)
    {
        var card = new Card
        {
            Name = cardDTO.Name,
            Description = cardDTO.Description,
            SeriesId = cardDTO.SeriesId,
            AssetsUrls = cardDTO.AssetsUrls,
            Status = cardDTO.Status,
        };
        _context.Cards.Add(card);
        await _context.SaveChangesAsync();

        return card;
    }
    public async Task<string?> GetRandomCardAssetUrl()
    {
        return await _context.Cards
            .Where(c => c.AssetsUrls.Count > 0 && c.Status == SuggestionStatus.APPROVED)
            .SelectMany(c => c.AssetsUrls)
            .OrderBy(o => Guid.NewGuid())
            .FirstOrDefaultAsync();
    }
    public async Task<List<Pack>> GetAllAvailablePacks()
    {
        return await _context.Packs
            .Where(p => p.Active)
            .ToListAsync();
    }
    public async Task<List<UserCardDTO>> GetUserCollection(long guildId, long userId)
    {
        var userCards = await _context.UserCards
            .Include(uc => uc.Card)
            .ThenInclude(c => c!.Series)
            .Where(uc =>
                uc.UserId == userId &&
                uc.GuildId == guildId &&
                uc.Amount > 0)
            .ToListAsync();

        return [.. userCards.Select(uc => MapToDTO(uc))];
    }
    public async Task<int> UpdatePacksCount(long guildId, long userId, int packId, int amount)
    {
        return await _packService.UpdateUserPackAmount(guildId, userId, packId, amount);
    }
    public async Task<List<UserCardDTO>> GetCardsFromPack(long guildId, long userId, int packId, int amount)
    {
        await _packService.UpdateUserPackAmount(guildId, userId, packId, -amount);

        int cardsInPackAmount = int.Parse(_config["CCG:CardsInPackAmount"]!);
        int totalCardsAmount = cardsInPackAmount * amount;
        var cardsIds = await _packService.GetRandomPackCardsIds(packId, totalCardsAmount);

        var drops = await ProcessPackCardIds(guildId, userId, cardsIds);
        return drops;
    }
    private async Task<List<UserCardDTO>> ProcessPackCardIds(
        long guildId, long userId, List<int> cardIds)
    {
        var userCards = await _context.UserCards
            .Include(uc => uc.Card)
            .ThenInclude(c => c!.Series)
            .Where(us =>
                us.UserId == userId &&
                us.GuildId == guildId)
            .ToListAsync();

        var dbCards = await _context.Cards
            .Include(c => c.Series)
            .Where(c => cardIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id);

        var drops = new List<UserCardDTO>();
        foreach (var id in cardIds)
        {
            var userCard = userCards.FirstOrDefault(uc => uc.Card!.Id == id);

            if (userCard != null)
            {
                userCard.Amount++;
            }
            else
            {
                if (!dbCards.TryGetValue(id, out var card))
                    continue;

                userCard = new UserCard
                {
                    GuildId = guildId,
                    UserId = userId,
                    Card = card,
                };
                _context.UserCards.Add(userCard);
                userCards.Add(userCard);
            }

            drops.Add(MapToDTO(userCard));
        }

        await _context.SaveChangesAsync();
        return drops;
    }
    private static UserCardDTO MapToDTO(UserCard userCard)
    {
        return new UserCardDTO
        {
            GuildId = userCard.GuildId,
            UserId = userCard.UserId,
            Id = userCard.CardId,
            Name = userCard.Card!.Name,
            Description = userCard.Card.Description,
            Series = userCard.Card.Series,
            SeriesId = userCard.Card.SeriesId,
            AssetsUrls = userCard.Card.AssetsUrls ?? [],
            AssetIndex = userCard.AssetIndex,
            Amount = userCard.Amount,
        };
    }

}