using LinguiCards.Application.Common.Exceptions.Base;

namespace LinguiCards.Application.Common.Exceptions;

public class LanguageAlreadyExistsException: BadRequestException
{
    public LanguageAlreadyExistsException()
        : base("Language already exists")
    {
    }
}