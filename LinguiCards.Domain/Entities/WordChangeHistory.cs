namespace LinguiCards.Domain.Entities;

public class WordChangeHistory
{
    public int Id { get; set; }
    public bool IsCorrectAnswer { get; set; }
    public double PassiveLearned { get; set; }
    public double ActiveLearned { get; set; }
    public int VocabularyType { get; set; }
    
    public Guid? TrainingId { get; set; }
    
    public string? CorrectAnswer { get; set; }
    
    public string? Answer { get; set; }

    public DateTime ChangedOn { get; set; }

    public int WordId { get; set; }

    public virtual Word Word { get; set; }
}