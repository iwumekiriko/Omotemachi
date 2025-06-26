namespace Omotemachi.Models.V1.Statistics;

public class VoiceStatistics : BaseStatistics
{
    public int VoiceTimeMuted { get; set; }
    public int VoiceTimeUnMuted { get; set; }

    public override object GetStats()
    {
        return new { VoiceTimeMuted, VoiceTimeUnMuted };
    }
}