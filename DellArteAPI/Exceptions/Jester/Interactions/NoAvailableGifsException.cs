using DellArteAPI.Models.V1.Jester.Interactions;

namespace DellArteAPI.Exceptions.Jester.Interactions;

public class NoAvailableGifsException(
    InteractionsActions action,
    InteractionsTypes type
) : Exception, ICustomException
{
    public string Code { get; set; } = "00622";
    public InteractionsActions Action { get; set; } = action;
    public InteractionsTypes Type { get; set; } = type;
}