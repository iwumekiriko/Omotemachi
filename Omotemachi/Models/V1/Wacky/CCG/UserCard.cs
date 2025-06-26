using Omotemachi.Tools;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Wacky.CCG;

public class UserCard
{
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }

    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }

    public int CardId { get; set; }
    [ForeignKey("CardId")]
    public Card? Card { get; set; }

    public int Amount { get; set; } = 1;

    public int AssetIndex { get; set; } = 0;
    public DateTimeOffset AcquiredAt { get; set; } = TimeConverter.GetCurrentTime();

    public string? PreferredAssetUrl => Card?.AssetsUrls?[AssetIndex];
}
