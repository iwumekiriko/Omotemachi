namespace DellArteAPI.Exceptions.Jester.Duets;

public class DisposeRestrictedException : Exception, ICustomException
{
    public string Code { get; set; } = "80038";
}
