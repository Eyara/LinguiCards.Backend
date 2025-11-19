namespace LinguiCards.Application.Common.Models;

public class TranslationEvaluationHistoryDTO
{
    public int Id { get; set; }
    public string OriginalText { get; set; }
    public string UserTranslation { get; set; }
    public string CorrectTranslation { get; set; }
    
    public int Accuracy { get; set; }
    public string Level { get; set; }
    
    public string MinorIssues { get; set; }
    public string Errors { get; set; }
    public string CriticalErrors { get; set; }
    public int UserId { get; set; }
    public int? LanguageId { get; set; }
}