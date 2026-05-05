using Omotemachi.Models.V1.Domain;

namespace Omotemachi.Models.V1.DTOs.Wacky;

public class AppaDTO
{
    public long GuildId { get; set; }
    public Guild? Guild { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string AssetUrl { get; set; }
    public int Amount { get; set; }
}