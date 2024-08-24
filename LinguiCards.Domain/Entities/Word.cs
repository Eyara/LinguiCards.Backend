namespace LinguiCards.Domain.Entities;

public class Word
{
    public int Id { get; set; }
    public string Name { get; set; }

    public double LearnedPercent { get; set; }
    public DateTime? LastUpdated { get; set; }

    public int LanguageId { get; set; }

    public virtual Language Language { get; set; }
}