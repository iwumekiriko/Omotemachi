using Omotemachi.Models.V1.Domain.Wacky.CCG;

namespace Omotemachi.Models.V1.DTOs.Wacky;

public class CardDTO
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int SeriesId { get; set; }
    public Series? Series { get; set; }
    public List<string> AssetsUrls { get; set; } = [];
    public SuggestionStatus Status { get; set; }
}
