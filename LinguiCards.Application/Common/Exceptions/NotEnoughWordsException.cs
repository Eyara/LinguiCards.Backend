using LinguiCards.Application.Common.Exceptions.Base;

namespace LinguiCards.Application.Common.Exceptions;

public class NotEnoughWordsException: BadRequestException
{
    public NotEnoughWordsException()
        : base("There are not enough words in current language. Please, make sure that you have at least 4 words to train.")
    {
    }
}