using Omotemachi.Tools;

namespace Omotemachi.Models.V1.Wacky.CCG;

public class Series
{
    public int Id {  get; set; }
    public required string Name { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = TimeConverter.GetCurrentTime();
}
