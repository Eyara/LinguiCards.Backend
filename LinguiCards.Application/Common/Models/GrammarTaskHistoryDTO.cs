namespace LinguiCards.Application.Common.Models;

public class GrammarTaskHistoryDTO
{
    public Guid Id { get; set; }
    
    public string ExpectedAnswer { get; set; }
    public string UserAnswer { get; set; }
    public string Explanation { get; set; }
    
    public int Accuracy { get; set; }
    public string Level { get; set; }
    public string Type { get; set; }
    public string Topic { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public int UserId { get; set; }
    public int? LanguageId { get; set; }
}

