using LinguiCards.Application.Common.Exceptions.Base;

namespace LinguiCards.Application.Common.Exceptions;

public class WordAlreadyExistsException : BadRequestException
{
    public WordAlreadyExistsException()
        : base("Word already exists")
    {
    }
}