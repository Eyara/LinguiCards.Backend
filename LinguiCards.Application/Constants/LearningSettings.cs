namespace LinguiCards.Application.Constants;

public static class LearningSettings
{
    public static readonly int LearnThreshold = 85;
    public static readonly int SuccessXpStep = 5;
    public static readonly int FailXpStep = 1;

    public static int LanguageLevelToNumber(string level)
    {
        return level.ToUpper() switch
        {
            "A1" => 1,
            "A2" => 2,
            "B1" => 3,
            "B2" => 4,
            "C1" => 5,
            "C2" => 6,
            _ => throw new ArgumentException("Unknown language level")
        };
    }
}