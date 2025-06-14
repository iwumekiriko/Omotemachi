using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.Models.V1.Wacky.CCG;

public class Pack
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsSeriesSpecific { get; set; } = false;
    public int? SeriesId { get; set; }
    [ForeignKey("SeriesId")]
    public Series? SpecificSeries { get; set; } = null;

    public required ICollection<Card> Cards { get; set; }
}
