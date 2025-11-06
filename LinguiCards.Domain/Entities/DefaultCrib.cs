namespace LinguiCards.Domain.Entities;

public class DefaultCrib
{
    public int Id { get; set; }

    public int LanguageId { get; set; }
    public Language Language { get; set; }
}