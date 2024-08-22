namespace LinguiCards.Domain.Entities;

public class Language
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int CountryId { get; set; }
    public int UserId { get; set; }

    // public virtual Country Country { get; set; }
    // public virtual User User { get; set; }
    //
    // public ICollection<Word> Words { get; set; }
}