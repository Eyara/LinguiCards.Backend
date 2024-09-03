namespace LinguiCards.Domain.Entities;

public class LanguageDictionary
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }

    public virtual ICollection<Language> Languages { get; set; }
}