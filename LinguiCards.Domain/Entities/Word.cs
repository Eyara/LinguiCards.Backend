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

    public int LanguageId { get; set; }

    public virtual Language Language { get; set; }

    public ICollection<WordChangeHistory> Histories { get; set; }
}