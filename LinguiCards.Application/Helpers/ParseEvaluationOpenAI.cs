using System.Text.RegularExpressions;
using LinguiCards.Application.Common.Models.Integration;

namespace LinguiCards.Application.Helpers;

public static class ParseEvaluationOpenAI
{
    public static TranslationEvaluationDTO ParseEvaluation(string response)
    {
        var result = new TranslationEvaluationDTO();

        // Match the accuracy using regex
        var accuracyMatch = Regex.Match(response, @"Точность перевода:\s*(\d+)\s*%");
        if (accuracyMatch.Success)
        {
            result.Accuracy = int.Parse(accuracyMatch.Groups[1].Value);
        }

        // Match the correct translation
        var correctTranslationMatch = Regex.Match(response, @"Правильный перевод:\s*(.*?)\s*(?=Неточности:|Ошибки:|Грубые ошибки:|$)");
        if (correctTranslationMatch.Success)
        {
            result.CorrectTranslation = correctTranslationMatch.Groups[1].Value.Trim();
        }

        // Match the minor issues
        var minorIssuesMatch = Regex.Match(response, @"Неточности:\s*(.*?)\s*(?=Ошибки:|Грубые ошибки:|$)");
        if (minorIssuesMatch.Success)
        {
            var text = minorIssuesMatch.Groups[1].Value.Trim();
            result.MinorIssues = ParseIssueList(text);
        }

        // Match the errors
        var errorsMatch = Regex.Match(response, @"Ошибки:\s*(.*?)\s*(?=Грубые ошибки:|$)");
        if (errorsMatch.Success)
        {
            var text = errorsMatch.Groups[1].Value.Trim();
            result.Errors = ParseIssueList(text);
        }

        // Match the critical errors
        var criticalErrorsMatch = Regex.Match(response, @"Грубые ошибки:\s*(.*?)$");
        if (criticalErrorsMatch.Success)
        {
            var text = criticalErrorsMatch.Groups[1].Value.Trim();
            result.CriticalErrors = ParseIssueList(text);
        }

        return result;
    }

    private static List<string> ParseIssueList(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Equals("нет", StringComparison.OrdinalIgnoreCase))
            return new List<string>();

        return input.Split(',')
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }
}
