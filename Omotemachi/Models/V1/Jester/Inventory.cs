using Omotemachi.Models.V1.Jester.InventoryItems;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester;

public class Inventory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InventoryId { get; set; }
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild Guild { get; set; }
    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }

    public ICollection<InventoryRole> InventoryRoles { get; set; } = [];
    public ICollection<InventoryExpBooster> InventoryExpBoosters { get; set; } = [];
    public ICollection<InventoryLootboxKey> InventoryLootboxKeys { get; set; } = [];
}