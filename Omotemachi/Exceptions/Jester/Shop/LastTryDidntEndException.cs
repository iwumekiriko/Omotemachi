namespace Omotemachi.Exceptions.Shop;

public class LastTryDidntEndException(long willEndAt) : Exception, ICustomException
{
    public string Code { get; set; } = "09922";
    public long WillEndAt { get; set; } = willEndAt;
}
