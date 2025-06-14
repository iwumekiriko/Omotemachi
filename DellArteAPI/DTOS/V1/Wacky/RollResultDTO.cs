using SixLabors.ImageSharp.PixelFormats;

namespace DellArteAPI.DTOS.V1;

public class RollParameters
{
    public int Throws { get; set; }
    public int Sides { get; set; }
    public int Modifier { get; set; }
    public string OriginalExpression { get; set; }
}

public class RollResultDTO
{
    public RollParameters Parameters { get; set; }
    public List<int> Rolls { get; set; }
    public int RollsSum => Rolls.Sum();
    public int Total => RollsSum + Parameters.Modifier;
}