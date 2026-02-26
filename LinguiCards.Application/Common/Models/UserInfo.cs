namespace LinguiCards.Application.Common.Models;

public class UserInfo
{
    public int Level { get; set; }
    public double Xp { get; set; }
    public double XpToNextLevel { get; set; }
    public int DailyXp { get; set; }
    public int ByTranslation { get; set; }
    public int ByGrammar { get; set; }
    public int GoalStreak { get; set; }
    public List<GoalDay> GoalDays { get; set; }
    public List<LanguageStat> LanguageStats { get; set; }
}

public class GoalDay
{
    public DateOnly? Date { get; set; }
    public int Xp { get; set; }
    public int TargetXp { get; set; }
    public int ByTranslation { get; set; }
    public int ByGrammar { get; set; }
    public bool IsCompleted { get; set; }
}

public class LanguageStat
{
    public string LanguageName { get; set; }
    public int TotalWords { get; set; }
    public int LearnedWords { get; set; }
    public double LearnedPercent { get; set; }
    public int TotalTrainingDays { get; set; }
}