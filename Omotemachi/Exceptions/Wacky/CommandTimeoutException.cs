using NuGet.Packaging.Signing;

namespace Omotemachi.Exceptions.Wacky;

public class CommandTimeoutException(
    TimeSpan timeLeft
) : Exception, ICustomException
{
    public string Code { get; set; } = "00f87";
    public TimeSpan TimeLeft { get; set; } = timeLeft;
}