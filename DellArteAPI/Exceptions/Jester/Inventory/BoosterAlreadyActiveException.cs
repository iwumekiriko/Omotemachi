namespace DellArteAPI.Exceptions.Jester.Inventory;

public class BoosterAlreadyActiveException(
    int remaining
) : Exception, ICustomException
{
    public string Code { get; set; } = "00059";
    public int Remaining { get; set; } = remaining;
}
