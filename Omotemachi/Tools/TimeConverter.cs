namespace Omotemachi.Tools;

public class TimeConverter
{
    private static readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");

    public static DateTimeOffset GetCurrentTime() 
    {
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;
        return TimeZoneInfo.ConvertTime(utcNow, _timeZone);
    }
}
