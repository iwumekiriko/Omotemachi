using DellArteAPI.Models.V1;

namespace DellArteAPI.Services.Jester;

public interface IEconomyService
{
    Task<Member> CreateCoinsTransaction(long guildId, long payerId, long recipientId, int amount);
    Task<Member> CreateCrystalsTransaction(long guildId, long payerId, long recipientId, int amount);
}

public class EconomyService(
    AppContext context,
    ILogger<EconomyService> logger,
    IMembersService memberService
) : ServiceBase(context, logger), IEconomyService
{
    private readonly IMembersService _memberService = memberService;

    public async Task<Member> CreateCoinsTransaction(
        long guildId, long payerId, long recipientId, int amount)
    {
        var payerData = await UpdateMembersCoins(guildId, payerId, recipientId, amount);
        await CreateTransaction(guildId, payerId, recipientId, amount, TransactionTypes.Coins);

        return payerData;
    }
    public async Task<Member> CreateCrystalsTransaction(
        long guildId, long payerId, long recipientId, int amount)
    {
        var payerData = await UpdateMembersCrystals(guildId, payerId, recipientId, amount);
        await CreateTransaction(guildId, payerId, recipientId, amount, TransactionTypes.Crystals);

        return payerData;
    }
    private async Task CreateTransaction(
        long guildId, long payerId, long recipientId, int amount, TransactionTypes type)
    {
        TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;
        var transtactionDate = TimeZoneInfo.ConvertTime(utcNow, moscowTimeZone);

        var transaction = new Transaction
        {
            GuildId = guildId,
            PayerId = payerId,
            RecipientId = recipientId,
            Amount = amount,
            Type = type,
            Date = transtactionDate
        };
        _context.Add(transaction);
        await _context.SaveChangesAsync();
    }
    private async Task<Member> UpdateMembersCoins(
        long guildId, long payerId, long recipientId, int amount)
    {
        var payerData = await _memberService.UpdateCoinsAsync(guildId, payerId, -amount);
        await _memberService.UpdateCoinsAsync(guildId, recipientId, amount);

        return payerData;
    }
    private async Task<Member> UpdateMembersCrystals(
        long guildId, long payerId, long recipientId, int amount)
    {
        var payerData = await _memberService.UpdateCrystalsAsync(guildId, payerId, -amount);
        await _memberService.UpdateCrystalsAsync(guildId, recipientId, amount);

        return payerData;
    }
}
