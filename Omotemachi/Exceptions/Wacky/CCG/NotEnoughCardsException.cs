namespace Omotemachi.Exceptions.Wacky.CCG;

public class NotEnoughCardsException(
    string cardName,
    string cardSeriesName,
    int cardsAmount,
    int cardsNeeded
) : Exception, ICustomException
{
    public string Code { get; set; } = "00f15";
    public string Name { get; set; } = cardName;
    public string SeriesName { get; set; } = cardSeriesName;
    public int Amount { get; set; } = cardsAmount;
    public int Needed { get; set; } = cardsNeeded;
}
