namespace LinguiCards.Application.Common.Models;

public class WordChangeHistoryDTO
{
    public int Id { get; set; }
    public bool IsCorrectAnswer { get; set; }
    public double PassiveLearned { get; set; }
    public double ActiveLearned { get; set; }
    public int VocabularyType { get; set; }
    public DateTime ChangedOn { get; set; }
}