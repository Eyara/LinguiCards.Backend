namespace LinguiCards.Application.Common.Models;

public class WordDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string TranslatedName { get; set; }

    public double LearnedPercent { get; set; }
    public DateTime? LastUpdated { get; set; }

    public int LanguageId { get; set; }
}