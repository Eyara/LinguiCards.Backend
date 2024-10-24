namespace LinguiCards.Application.Common.Exceptions.Base;

public class BadRequestException : Exception
{
    public BadRequestException(string message = "Bad request. Verify your data before sending them again.") :
        base(message)
    {
    }
}