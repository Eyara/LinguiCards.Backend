using System.Text.RegularExpressions;

namespace LinguiCards.Application.Helpers;

public static class ParseGrammarTaskEvaluationOpenAi
{
    public static (string ExpectedAnswer, string Explanation) ParseEvaluation(string response)
    {
        var expectedAnswer = "";
        var explanation = "";

        var expectedAnswerMatch = Regex.Match(response, @"Правильный ответ:\s*(.*?)\s*(?=Объяснение:|$)", RegexOptions.IgnoreCase);
        if (expectedAnswerMatch.Success)
        {
            expectedAnswer = expectedAnswerMatch.Groups[1].Value.Trim();
            if (expectedAnswer.Equals("нет", StringComparison.OrdinalIgnoreCase))
            {
                expectedAnswer = "";
            }
        }

        var explanationMatch = Regex.Match(response, @"Объяснение:\s*(.*?)$", RegexOptions.IgnoreCase);
        if (explanationMatch.Success)
        {
            explanation = explanationMatch.Groups[1].Value.Trim();
            if (explanation.Equals("нет", StringComparison.OrdinalIgnoreCase))
            {
                explanation = "";
            }
        }

        if (string.IsNullOrWhiteSpace(explanation) && string.IsNullOrWhiteSpace(expectedAnswer))
        {
            explanation = response;
        }

        return (expectedAnswer, explanation);
    }
}

