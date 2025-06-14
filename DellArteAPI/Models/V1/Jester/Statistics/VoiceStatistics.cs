namespace DellArteAPI.Models.V1.Jester.Statistics;

public class VoiceStatistics : BaseStatistics
{
    public int VoiceTimeMuted { get; set; }
    public int VoiceTimeUnMuted { get; set; }

    public override object GetStats()
    {
        return new { VoiceTimeMuted, VoiceTimeUnMuted };
    }
}