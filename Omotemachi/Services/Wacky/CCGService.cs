using Omotemachi.DTOS.V1.Wacky;
using Omotemachi.Models.V1.Wacky.CCG;
using Microsoft.EntityFrameworkCore;
using Omotemachi.Exceptions.Wacky.CCG;
using Omotemachi.Models.V1.Statistics;
using Omotemachi.Tools;
using Omotemachi.Models.V1;
using NuGet.Protocol;

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
    Task<string?> GetRandomCardAssetUrlAsync();
    Task<List<PackDTO>> GetAllAvailablePacksAsync(long guildId, long userId);
    Task<List<PackDTO>> GetAllAvailablePacksByNameAsync(long guildId, long userId, string? input);
    Task<List<UserCardDTO>> GetUserCollectionAsync(long guildId, long userId);
    Task<List<UserCardDTO>> GetUserCardsByNameAsync(long guildId, long userId, string? input);
    Task<UserCardDTO?> GetUserCardDTOByIdAsync(long guildId, long userId, int id);
    Task GiveCardsToMemberAsync(long guildId, long userId, long receiverId, int cardId, int amount);
    Task UpdateUserCardAssetIndexAsync(long guildId, long userId, int cardId, int index);
    Task<int> UpdatePacksCountAsync(long guildId, long userId, int packId, int amount);
    Task<int> UpdateUserCardAmount(long guildId, long userId, int cardId, int amount);
    Task<List<UserCardDTO>> GetCardsFromPackAsync(long guildId, long userId, int packId, int packAmount);
}
public class CCGService(
    AppContext context,
    ILogger<CCGService> logger,
    IConfiguration config,
    IPacksService packService,
    IStatisticsService statistics
) : ServiceBase(context, logger), ICCGService
{
    private readonly IConfiguration _config = config;
    private readonly IPacksService _packService = packService;
    private readonly IStatisticsService _statistics = statistics;
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
    public async Task<List<UserCardDTO>> GetUserCardsByNameAsync(long guildId, long userId, string? input)
    {
        input = input?.ToLower();

        return await _context.UserCards
            .Include(uc => uc.Card)
            .ThenInclude(uc => uc!.Series)
            .Where(uc =>
                EF.Functions.Like(uc.Card!.Name.ToLower(), $"%{input}%") &&
                uc.GuildId == guildId &&
                uc.UserId == userId &&
                uc.Amount > 0
            )
            .Take(25)
            .Select(uc => MapToDTO(uc))
            .ToListAsync();
    }
    public async Task<UserCardDTO?> GetUserCardDTOByIdAsync(long guildId, long userId, int id)
    {
        var uCard = await GetUserCardByIdAsync(guildId, userId, id);

        if (uCard == null)
            return null;

        return MapToDTO(uCard);
    }
    private async Task<UserCard?> GetUserCardByIdAsync(long guildId, long userId, int id)
    {
        return await _context.UserCards
            .Include(uc => uc.Card)
            .ThenInclude(uc => uc!.Series)
            .Where(uc =>
                uc.GuildId == guildId &&
                uc.UserId == userId &&
                uc.CardId == id)
            .FirstOrDefaultAsync();
    }
    public async Task UpdateUserCardAssetIndexAsync(long guildId, long userId, int cardId, int index)
    {
        var uCard = await GetUserCardByIdAsync(guildId, userId, cardId);
        if (uCard == null) return;

        uCard.AssetIndex = index;
        await _context.SaveChangesAsync();
    }
    public async Task GiveCardsToMemberAsync(long guildId, long userId, long receiverId, int cardId, int amount)
    {
        var timeoutData = await _context.TimeoutCardCatches
             .Where(t => t.GuildId == guildId && t.UserId == userId)
             .FirstOrDefaultAsync();

        var timeSpent = TimeConverter.GetCurrentTime() - (timeoutData?.LastGive ?? DateTimeOffset.MinValue);
        var timeout = TimeSpan.FromHours(int.Parse(_config["CCG:GiveCommandHoursTimeout"]!));

        if (timeoutData != null && timeSpent <= timeout)
            throw new GiveCommandTimeoutException(
                timeLeft: timeout - timeSpent);

        await UpdateUserCardAmount(guildId, userId, cardId, -amount);
        await UpdateUserCardAmount(guildId, receiverId, cardId, amount);
        await _statistics.IncrementStatistics<CardsStatistics>(
            guildId, userId, s => s.CardsGiftedCount, amount);

        if (timeoutData == null)
        {
            timeoutData = new TimeoutCardCatch
            {
                GuildId = guildId,
                UserId = userId
            };
            _context.TimeoutCardCatches.Add(timeoutData);
        }
        else
        {
            timeoutData.LastGive = TimeConverter.GetCurrentTime();
        }
        await _context.SaveChangesAsync();
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
    public async Task<string?> GetRandomCardAssetUrlAsync()
    {
        return await _context.Cards
            .Where(c => c.AssetsUrls.Count > 0 && c.Status == SuggestionStatus.APPROVED)
            .SelectMany(c => c.AssetsUrls)
            .OrderBy(o => Guid.NewGuid())
            .FirstOrDefaultAsync();
    }
    public async Task<List<PackDTO>> GetAllAvailablePacksAsync(long guildId, long userId)
    {
        return await _packService.GetAllAvailablePacks(guildId, userId, null);
    }
    public async Task<List<PackDTO>> GetAllAvailablePacksByNameAsync(long guildId, long userId, string? input)
    {
        return await _packService.GetAllAvailablePacks(guildId, userId, input);
    }
    public async Task<List<UserCardDTO>> GetUserCollectionAsync(long guildId, long userId)
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
    public async Task<List<UserCardDTO>> GetCardsFromPackAsync(long guildId, long userId, int packId, int amount)
    {
        await _packService.UpdateUserPackAmount(guildId, userId, packId, -amount);

        int cardsInPackAmount = int.Parse(_config["CCG:CardsInPackAmount"]!);
        int totalCardsAmount = cardsInPackAmount * amount;
        var cardsIds = await _packService.GetRandomPackCardsIds(packId, totalCardsAmount);

        var drops = await ProcessPackCardIds(guildId, userId, cardsIds);
        await _statistics.IncrementStatistics<CardsStatistics>(
            guildId, userId, s => s.PacksOpenedCount, amount);
        return drops;
    }
    public async Task<int> UpdatePacksCountAsync(long guildId, long userId, int packId, int amount)
    {
        return await _packService.UpdateUserPackAmount(guildId, userId, packId, amount);
    }
    public async Task<int> UpdateUserCardAmount(long guildId, long userId, int cardId, int amount)
    {
        var uCard = await GetUserCardByIdAsync(guildId, userId, cardId);

        if (uCard == null)
        {
            if (!await _context.Cards.AnyAsync(c => c.Id == cardId))
                throw new ArgumentException($"No card with id: {cardId}");

            uCard = new UserCard
            {
                GuildId = guildId,
                UserId = userId,
                CardId = cardId,
                Amount = 0
            };
            _context.UserCards.Add(uCard);
        }

        uCard.Amount += amount;
        if (uCard.Amount < 0)
        {
            var (cardName, cardSeriesName)= await _context.Cards
                .Include(c => c.Series)
                .Where(c => c.Id == cardId)
                .Select(c => new Tuple<string, string>(c.Name, c.Series!.Name))
                .FirstAsync();

            throw new NotEnoughCardsException(
                cardName: cardName,
                cardSeriesName: cardSeriesName,
                cardsAmount: uCard.Amount - amount,
                cardsNeeded: Math.Abs(amount)
            );
        }

        await _context.SaveChangesAsync();
        return uCard.Amount;
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