namespace LinguiCards.Application.Common.Models.Statistics;

public class LearningVelocityResponse
{
    public List<WeeklyVelocity> Weeks { get; set; } = new();
}

public class WeeklyVelocity
{
    public DateTime WeekStart { get; set; }
    public int WordsAdded { get; set; }
    public int WordsLearned { get; set; }
}
