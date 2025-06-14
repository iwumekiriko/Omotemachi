using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.Models.V1.Jester.Lootboxes;

public class LootboxRole
{
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild Guild { get; set; }
    public LootboxTypes LootboxType { get; set; }
    public long GuildRoleId { get; set; }
    public bool Exclusive { get; set; } = false;
    [NotMapped]
    public bool? GotByUser { get; set; }
}
