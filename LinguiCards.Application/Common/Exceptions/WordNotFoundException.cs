using LinguiCards.Application.Common.Exceptions.Base;

namespace LinguiCards.Application.Common.Exceptions;

public class WordNotFoundException : NotFoundException
{
    public WordNotFoundException()
        : base("Word not found")
    {
    }
}