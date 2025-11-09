namespace LinguiCards.Application.Constants;

public static class GrammarTaskPrompts
{
    private const string GrammarTaskPrompt = @"";

    private const string EvaluationGrammarTaskPrompt = @"";

    public static string GetGrammarTaskPrompt(string language, string level, string? topic, string? type)
    {
        return string.Format(GrammarTaskPrompt, language, level, topic ?? "", type ?? "");
    }

    public static string GetEvaluationGrammarTaskPrompt(string language, string level, string taskText, string userAnswer, string? topic, string? type)
    {
        return string.Format(EvaluationGrammarTaskPrompt, language, level, taskText, userAnswer, topic ?? "", type ?? "");
    }
}

