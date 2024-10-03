namespace LinguiCards.Application.Constants;

// TODO: brainstorm about moving it to User Settings in db
public static class LearningSettings
{
    public static readonly int LearnStep = 10;
    public static readonly int LearnThreshold = 85;
    public static readonly int SuccessXpStep = 5;
    public static readonly int FailXpStep = 1;
    public static double DayWeight = 0.3f;
    
}