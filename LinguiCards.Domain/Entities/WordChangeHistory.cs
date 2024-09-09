namespace LinguiCards.Domain.Entities;

public class WordChangeHistory
{
    public int Id { get; set; }
    public bool IsCorrectAnswer { get; set; }
    public DateTime ChangedOn { get; set; }

    public int WordId { get; set; }

    public virtual Word Word { get; set; }
}