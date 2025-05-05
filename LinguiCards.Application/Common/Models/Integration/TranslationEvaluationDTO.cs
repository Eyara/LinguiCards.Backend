namespace LinguiCards.Application.Common.Models.Integration;

public class TranslationEvaluationDTO
{
    public int Accuracy { get; set; }
    public string CorrectTranslation { get; set; } = string.Empty;
    public List<string> MinorIssues { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public List<string> CriticalErrors { get; set; } = new();
}
    