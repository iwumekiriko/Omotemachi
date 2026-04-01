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
            var now = GetCurrentTime();
            return new DateTimeOffset(
                now.Year, now.Month, now.Day,
                0, 0, 0,
                now.Offset
            );
        }
    }
    public static DateTimeOffset Tomorrow
    {
        get => Today.AddDays(1);
    }
}
