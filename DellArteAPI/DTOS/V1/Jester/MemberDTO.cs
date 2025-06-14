using DellArteAPI.Models.V1;

namespace DellArteAPI.DTOS.V1.Jester;

public class MemberDTO
{
    public long GuildId { get; set; }
    public Guild? Guild { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }

    public bool Active { get; set; } = true;
    public int Experience { get; set; } = 0;
    public int ExpMultiplier { get; set; } = 1;
    public int Coins { get; set; } = 0;
    public int Crystals { get; set; } = 0;
    public int MessageCount { get; set; } = 0;
    public long VoiceTime { get; set; } = 0;
    public DateTime? JoinedAt { get; set; } = DateTime.Now;
    public bool IsBot { get; set; } = false;
}
