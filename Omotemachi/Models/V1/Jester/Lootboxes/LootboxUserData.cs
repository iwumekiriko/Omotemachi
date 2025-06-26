using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester.Lootboxes;

public class LootboxUserData
{
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }
    public LootboxTypes LootboxType { get; set; }
    public Dictionary<string, int> Data { get; set; } = [];
}