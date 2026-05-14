using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Domain.Jester.Items;

public class ExpBooster : Item
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ExpBoosterId { get; set; }
    public int Value { get; set; }
    public int Duration { get; set; }
}
