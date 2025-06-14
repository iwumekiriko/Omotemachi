namespace DellArteAPI.Exceptions.Jester.Members;

public class NotEnoughCrystalsException(
    int current, int needed
) : Exception, ICustomException
{
    public int Current { get; } = current;
    public int Needed { get; } = needed;
    public string Code { get; } = "00318";
}