namespace LinguiCards.Application.Common.Models;

public class WordDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string TranslatedName { get; set; }

    public double PassiveLearnedPercent { get; set; }
    public double ActiveLearnedPercent { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastUpdated { get; set; }

    public double PassiveEaseFactor { get; set; }
    public int PassiveIntervalDays { get; set; }
    public int PassiveRepetitionCount { get; set; }
    public DateTime? PassiveNextReviewDate { get; set; }

    public double ActiveEaseFactor { get; set; }
    public int ActiveIntervalDays { get; set; }
    public int ActiveRepetitionCount { get; set; }
    public DateTime? ActiveNextReviewDate { get; set; }

    public int LanguageId { get; set; }
}

public class WordExtendedDTO : WordDto
{
    public List<WordChangeHistoryDTO> Histories { get; set; }
}

public enum VocabularyType
{
    Passive = 0,
    Active
}