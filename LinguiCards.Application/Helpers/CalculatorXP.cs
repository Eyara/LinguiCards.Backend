namespace LinguiCards.Application.Helpers;

public static class CalculatorXP
{
    private const int BaseXp = 100;
    private const int Increment = 50;
    
    public static int CalculateXpRequired(int level)
    {
        var multiplier = level switch
        {
            <= 10 => 1.1,
            <= 20 => 1.5,
            <= 30 => 2.0,
            <= 40 => 2.5,
            <= 50 => 3.0,
            <= 60 => 3.5,
            <= 70 => 5.0,
            <= 80 => 8.0,
            _ => 10.0
        };

        return (int)((BaseXp + (Increment * (level - 1))) * Math.Log(level + 1) * multiplier);
    }
}