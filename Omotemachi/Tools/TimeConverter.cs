namespace Omotemachi.Tools;

public class TimeConverter
{
    private static readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");

    public static DateTimeOffset GetCurrentTime() 
    {
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;
        return TimeZoneInfo.ConvertTime(utcNow, _timeZone);
    }
    public static DateTimeOffset Today
    {
        get 
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow.Date;
            return TimeZoneInfo.ConvertTime(utcNow, _timeZone);
        }
    }
    public static DateTimeOffset Tomorrow
    {
        get => Today.AddDays(1);
    }
}
