using Omotemachi.Models.V1.Domain;
using Omotemachi.Models.V1.Domain.Wacky.CCG;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.DTOs.Wacky;

public class UserCardDTO
{
    public long GuildId { get; set; }
    public Guild? Guild { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int SeriesId { get; set; }
    public Series? Series { get; set; }
    public List<string> AssetsUrls { get; set; } = [];
    public int AssetIndex { get; set; } = 0;
    public int Amount { get; set; }
}
