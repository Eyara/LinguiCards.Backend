namespace LinguiCards.Domain.Entities;

public class Word
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string TranslatedName { get; set; }

    public double PassiveLearnedPercent { get; set; }
    public double ActiveLearnedPercent { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastUpdated { get; set; }

    public double PassiveEaseFactor { get; set; } = 2.5;
    public int PassiveIntervalDays { get; set; }
    public int PassiveRepetitionCount { get; set; }
    public DateTime? PassiveNextReviewDate { get; set; }

    public double ActiveEaseFactor { get; set; } = 2.5;
    public int ActiveIntervalDays { get; set; }
    public int ActiveRepetitionCount { get; set; }
    public DateTime? ActiveNextReviewDate { get; set; }

    public int LanguageId { get; set; }

    public Language Language { get; set; }

    public ICollection<WordChangeHistory> Histories { get; set; }
}