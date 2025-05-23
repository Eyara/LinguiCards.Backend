namespace LinguiCards.Application.Helpers;

public static class RedisKeyHelper
{
    public static string GetWordOfTheDayKey(int userId, int languageId)
    {
        return $"word_of_the_day:{userId}:{languageId}";
    }
}
