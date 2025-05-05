namespace LinguiCards.Application.Constants;

public static class TranslationPrompts
{
    private const string GetTextPrompt =
        @"Сыграем в игру. Я даю тебе язык, размер, уровень и тему текста. Ты мне выдаешь текст согласно условиям, БЕЗ ДОПОЛНИТЕЛЬНЫХ СЛОВ. {0}. {1} предложение. Уровень {2}. Тема: {3}.";

    private const string EvaluationTranslationPrompt = @"
        Сыграем в игру. Я даю тебе перевод, ты его оцениваешь, отвечая в формате: ""Точность перевода: N %. Правильный перевод: *перевод*. Неточности: *указываешь*. Ошибки: *указываешь*. Грубые ошибки: *указываешь*"". И ничего кроме этого!
        Если неточностей или ошибок нет - пиши ""нет"" в соответствующей графе.
        {0}. Уровень {1}.        
        Оригинальный текст: {2}
        Перевод: {3}
    """;

    public static string GetStartGamePrompt(string language, int sentenceCount, string level, string topic)
    {
        return string.Format(GetTextPrompt, language, sentenceCount, level, topic);
    }

    public static string GetEvaluationTranslationPrompt(string language, string level, string originalText,
        string translation)
    {
        return string.Format(EvaluationTranslationPrompt, language, level, originalText, translation);
    }
}