namespace LinguiCards.Domain.Entities;

public class DefaultCrib
{
    public int Id { get; set; }

    public int LanguageId { get; set; }
    public virtual Language Language { get; set; }
}