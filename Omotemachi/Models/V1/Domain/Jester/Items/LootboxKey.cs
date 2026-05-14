using Omotemachi.Models.V1.Domain.Jester.Lootboxes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Domain.Jester.Items;

public class LootboxKey : Item
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LootboxKeyId { get; set; }
    public LootboxTypes Type { get; set; }
}