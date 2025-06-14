using DellArteAPI.Models.V1.Wacky.CCG;
using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.DTOS.V1.Wacky;

public class CardDTO
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int SeriesId { get; set; }
    public Series? Series { get; set; }
    public List<string> AssetsUrls { get; set; } = [];
    public SuggestionStatus Status { get; set; }
}
