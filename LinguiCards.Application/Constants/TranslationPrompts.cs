namespace LinguiCards.Application.Constants;

public static class TranslationPrompts
{
    private const string GetTextPrompt =
        @"Сгенерируй текст по заданным условиям. Без вступлений и пояснений.
Язык: {0}
Слов: {1}
Уровень: {2}
Тема: {3} (если пусто — игнорируй)";


    private const string EvaluationTranslationPrompt = @"
Оцени перевод. Ответ строго в следующем формате:
Точность перевода: N %. 
Правильный перевод: *укажи правильный вариант*. 
Неточности: *если есть, перечисли*. 
Ошибки: *если есть, перечисли*. 
Грубые ошибки: *если есть, перечисли*. 
Если чего-то нет — пиши ""нет"". Никакого дополнительного текста.

Язык: {0}. Уровень: {1}.        
Оригинальный текст: {2}
Перевод: {3}";


    public static string GetStartGamePrompt(string language, int wordLength, string level, string topic)
    {
        return string.Format(GetTextPrompt, language, wordLength, level, topic);
    }

    public static string GetEvaluationTranslationPrompt(string language, string level, string originalText,
        string translation)
    {
        return string.Format(EvaluationTranslationPrompt, language, level, originalText, translation);
    }
}