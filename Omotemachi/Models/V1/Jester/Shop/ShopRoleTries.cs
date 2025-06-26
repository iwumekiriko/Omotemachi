using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester.Shop;

public class ShopRoleTries
{
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild Guild { get; set; }
    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    public long GuildRoleId { get; set; }
    public int TriesUsed { get; set; } = 0;
    public long? TryActivated {  get; set; }
    public bool Active { get; set; } = false;
}
