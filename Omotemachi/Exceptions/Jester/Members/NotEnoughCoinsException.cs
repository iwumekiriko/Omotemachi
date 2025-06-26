namespace Omotemachi.Exceptions.Jester.Members;

public class NotEnoughCoinsException(
    int current, int needed
) : Exception, ICustomException
{
    public int Current { get; } = current;
    public int Needed { get; } = needed;
    public string Code { get; } = "00317";
}