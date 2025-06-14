using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.Models.V1;

public class Member
{
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }

    public bool Active { get; set; } = true;
    public int Experience { get; set; } = 0;
    public int ExpMultiplier { get; set; } = 1;
    public int Coins { get; set; } = 0;
    public int Crystals { get; set; } = 0;
    public DateTime? JoinedAt { get; set; } = DateTime.Now;
    public bool IsBot { get; set; } = false;
}