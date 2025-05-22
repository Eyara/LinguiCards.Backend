namespace LinguiCards.Application.Common.Models;

public class LanguageDashboardStat
{
    public double ActiveLearnedPercent { get; set; }
    public double PassiveLearnedPercent { get; set; }
    public double ActiveAverageAccuracy { get; set; }
    public double PassiveAverageAccuracy { get; set; }
    public double ActiveAverageLearnedPercent { get; set; }
    public double PassiveAverageLearnedPercent { get; set; }
    public int LearnedWords { get; set; }
    public int TotalWords { get; set; }
    public int TotalTrainingDays { get; set; }
    
    public string WordOfTheDay { get; set; }

    public List<string> BestActiveWordsByAccuracy { get; set; }
    public List<string> BestPassiveWordsByAccuracy { get; set; }
    public List<string> WorstActiveWordsByAccuracy { get; set; }
    public List<string> WorstPassiveWordsByAccuracy { get; set; }
}