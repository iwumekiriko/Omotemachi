namespace Omotemachi.Exceptions.Jester.Inventory;

public class NoActiveBoosterException : Exception, ICustomException
{
    public string Code { get; set; } = "00058";
}
