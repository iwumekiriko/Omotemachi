using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1;

public enum TransactionTypes
{
    Coins = 1,
    Crystals = 2
}


public class Transaction
{
    public int Id { get; set; }
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild Guild { get; set; }

    public long PayerId { get; set; }
    [ForeignKey("PayerId")]
    public User Payer { get; set; }

    public long RecipientId { get; set; }
    [ForeignKey("RecipientId")]
    public User Recipient { get; set; }

    public int Amount { get; set; }
    public TransactionTypes Type { get; set; }
    public DateTimeOffset Date { get; set; }
}
