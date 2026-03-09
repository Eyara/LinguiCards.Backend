namespace LinguiCards.Application.Common.Models;

public class DailyGoalStatusDto
{
    public string Username { get; set; }
    public bool IsGoalCompleted { get; set; }
    public int GainedXp { get; set; }
    public int TargetXp { get; set; }
}
