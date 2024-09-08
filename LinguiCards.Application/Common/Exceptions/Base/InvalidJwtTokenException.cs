namespace LinguiCards.Application.Common.Exceptions.Base;

public class InvalidJwtTokenException : Exception
{
    public InvalidJwtTokenException(string message = "The provided JWT token is invalid or has expired") : base(message)
    {
    }
}