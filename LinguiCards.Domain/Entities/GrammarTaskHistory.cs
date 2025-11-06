namespace LinguiCards.Domain.Entities;

public class GrammarTaskHistory
{
    public Guid Id { get; set; }
    
    public string ExpectedAnswer { get; set; }
    public string UserAnswer { get; set; }
    public string Explanation { get; set; }
    
    public string Level { get; set; }
    public string Type { get; set; }
    public string Topic { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; }
    public User User { get; set; }
}
