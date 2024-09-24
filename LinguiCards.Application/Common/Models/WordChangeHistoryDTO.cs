namespace LinguiCards.Application.Common.Models;

public class WordChangeHistoryDTO
{
    public int Id { get; set; }
    public bool IsCorrectAnswer { get; set; }
    public DateTime ChangedOn { get; set; }
}