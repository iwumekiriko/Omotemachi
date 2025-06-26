using Omotemachi.Models.V1.Wacky.CCG;
using Omotemachi.Tools;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Wacky.CCG;

public class Card
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int SeriesId { get; set; }
    [ForeignKey("SeriesId")]
    public Series? Series { get; set; }
    public List<string> AssetsUrls { get; set; } = [];

    public SuggestionStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = TimeConverter.GetCurrentTime();
    public DateTimeOffset UpdatedAt { get; set; } = TimeConverter.GetCurrentTime();

    public ICollection<Pack> Packs { get; set; } = [];
}
