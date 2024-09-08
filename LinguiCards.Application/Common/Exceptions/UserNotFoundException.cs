using LinguiCards.Application.Common.Exceptions.Base;

namespace LinguiCards.Application.Common.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException()
        : base("User not found")
    {
    }
}