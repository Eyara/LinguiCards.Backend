namespace LinguiCards.Application.Common.Models.Integration;

public class GrammarTaskEvaluationDTO
{
    public int Accuracy { get; set; }
    public string ExpectedAnswer { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public List<string> MinorIssues { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public List<string> CriticalErrors { get; set; } = new();
}

