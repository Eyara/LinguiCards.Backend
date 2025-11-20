namespace LinguiCards.Domain.Entities;

public class Language
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }


    public int LanguageDictionaryId { get; set; }

    public LanguageDictionary LanguageDictionary { get; set; }

    public ICollection<Word> Words { get; set; }
    public ICollection<IrregularVerb> IrregularVerbs { get; set; }
}