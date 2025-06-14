using DellArteAPI.DTOS.V1.Wacky;
using DellArteAPI.Models.V1.Wacky.CCG;
using DellArteAPI.Tools;
using Microsoft.EntityFrameworkCore;

namespace DellArteAPI.Services.Wacky;

public interface ICCGService
{
    Task<List<Series>> GetSeriesAsync();
    Task<Series?> GetSeriesByIdAsync(int id);
    Task<Series?> GetSeriesByNameAsync(string name);
    Task<Series> CreateSeriesAsync(string name);
    Task<List<Card>> GetAllCardsAsync();
    Task<Card?> GetCardByIdAsync(int id);
    Task<Card> CreateCardAsync(CardDTO card);
    Task<List<UserCardDTO>> OpenCardsPacks(long guildId, long userId, int packAmount);
}
public class CCGService(
    AppContext context,
    ILogger<CCGService> logger,
    IConfiguration config
) : ServiceBase(context, logger), ICCGService
{
    private readonly IConfiguration _config = config;
    private readonly RandomGenerator _random = new((uint)Environment.TickCount);

    private List<Card> _cardsCache = [];
    private DateTime _lastCacheUpdate;

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
    public async Task<List<UserCardDTO>> OpenCardsPacks(long guildId, long userId, int packAmount)
    {
        int cardsInPackAmount = int.Parse(_config["CCG:CardsInPackAmount"]!);
        int totalCardsAmount = cardsInPackAmount * packAmount;
        var drops = await RandomizeCardsDrop(totalCardsAmount);

        var userCards = await _context.UserCards
            .Include(uc => uc.Card)
            .ThenInclude(c => c!.Series)
            .Where(us =>
                us.UserId == userId &&
                us.GuildId == guildId)
            .ToListAsync();

        var droppedUserCards = await ProcessCardDrops(guildId, userId, drops, userCards);
        return droppedUserCards;
    }
    private async Task<List<UserCardDTO>> ProcessCardDrops(
        long guildId, long userId, List<Card> drops, List<UserCard> existingUserCards)
    {
        var cardGroups = drops
            .GroupBy(c => c.Id)
            .ToDictionary(g => g.Key, g => g.Count());

        var droppedUserCards = new List<UserCardDTO>();
        foreach (var group in cardGroups)
        {
            var cardId = group.Key;
            var dropCount = group.Value;

            var userCard = existingUserCards.FirstOrDefault(uc => uc.Card!.Id == cardId);

            if (userCard != null)
            {
                userCard.Amount += dropCount;
            }
            else
            {
                var card = drops.First(c => c.Id == cardId);
                userCard = new UserCard
                {
                    GuildId = guildId,
                    UserId = userId,
                    CardId = card.Id,
                    Amount = dropCount,
                };
                _context.UserCards.Add(userCard);
                existingUserCards.Add(userCard);
            }

            droppedUserCards.Add(MapToDTO(userCard, dropCount));
        }

        await _context.SaveChangesAsync();
        return droppedUserCards;
    }
    private static UserCardDTO MapToDTO(UserCard userCard, int droppedCount)
    {
        return new UserCardDTO
        {
            GuildId = userCard.GuildId,
            UserId = userCard.UserId,
            Id = userCard.CardId,
            Name = userCard.Card!.Name,
            Description = userCard.Card.Description,
            SeriesId = userCard.Card.SeriesId,
            Series = userCard.Card.Series,
            AssetsUrls = userCard.Card.AssetsUrls ?? [],
            AssetIndex = userCard.AssetIndex,
            Amount = userCard.Amount,
            DroppedAmount = droppedCount
        };
    }
    private async Task<List<Card>> RandomizeCardsDrop(int amount)
    {
        if (_cardsCache.Count == 0 || (DateTime.Now - _lastCacheUpdate).TotalHours > 1)
        {
            _cardsCache = await _context.Cards.Include(c => c.Series).ToListAsync();
            _lastCacheUpdate = DateTime.Now;
        }
        var drops = new List<Card>(amount);
        for (int i = 0; i < amount; i++)
        {
            int index = _random.NextInt(0, _cardsCache.Count - 1);
            drops.Add(_cardsCache[index]);
        }

        return drops;
    }
}