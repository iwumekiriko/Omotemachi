using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester.Shop;

public class ShopRole
{
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild Guild { get; set; }
    public long GuildRoleId { get; set; }
    public bool Exclusive { get; set; } = false;
    public int Price { get; set; }
    [NotMapped]
    public bool? GotByUser { get; set; }
}
