namespace LinguiCards.Application.Common.Exceptions.Base;

public class EntityOwnershipException : Exception
{
    public EntityOwnershipException(string message = "The entity belongs to another user") : base(message)
    {
    }
}