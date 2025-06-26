using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Wacky.CCG;

public class Pack
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsSeriesSpecific { get; set; } = false;
    public int? SeriesId { get; set; } = null;
    [ForeignKey("SeriesId")]
    public Series? SpecificSeries { get; set; } = null;
    public bool Active { get; set; } = true;

    public ICollection<Card> Cards { get; set; } = [];
}
