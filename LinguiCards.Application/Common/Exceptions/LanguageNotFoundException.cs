using LinguiCards.Application.Common.Exceptions.Base;

namespace LinguiCards.Application.Common.Exceptions;

public class LanguageNotFoundException : NotFoundException
{
    public LanguageNotFoundException()
        : base("Language not found")
    {
    }
}