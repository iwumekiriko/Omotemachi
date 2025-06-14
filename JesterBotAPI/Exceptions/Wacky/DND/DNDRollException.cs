namespace DellArteAPI.Exceptions.Wacky.DND;

public class DNDRollException(string? message = "InvalidInputError") : Exception(message), ICustomException
{
    public string Code { get; set; } = "322pk";
}