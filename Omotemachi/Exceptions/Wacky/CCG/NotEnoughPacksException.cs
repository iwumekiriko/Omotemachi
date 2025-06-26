namespace Omotemachi.Exceptions.Wacky.CCG;

public class NotEnoughPacksException(
    string packName,
    int packsAmount,
    int packsNeeded
) : Exception, ICustomException
{
    public string Code { get; set; } = "00f14";
    public string Name { get; set; } = packName;
    public int Amount { get; set; } = packsAmount;
    public int Needed { get; set; } = packsNeeded;
}
