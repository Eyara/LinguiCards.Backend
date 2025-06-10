namespace LinguiCards.Application.Common.Models;

public class DailyGoalDTO
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public DateOnly Date { get; set; }

    public int TargetXp { get; set; }

    public int GainedXp { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}