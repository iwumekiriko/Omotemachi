using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.Models.V1.Jester.Items;

public class ActiveExpBooster : Item
{
    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    public int Value { get; set; }
    public int Duration { get; set; }
    public long ActivatedAt { get; set; }

}
