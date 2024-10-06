namespace LinguiCards.Application.Common.Models;

public class LanguageDashboardStat
{
    public int TotalWords { get; set; }
    public double ActiveLearnedPercent { get; set; }
    public double PassiveLearnedPercent { get; set; }
    public double ActiveAverageAccuracy { get; set; }
    public double PassiveAverageAccuracy { get; set; }
    public int TotalTrainingDays { get; set; }
    
    public List<string> BestActiveWordsByAccuracy { get; set; }
    public List<string> BestPassiveWordsByAccuracy { get; set; }
    public List<string> WorstActiveWordsByAccuracy { get; set; }
    public List<string> WorstPassiveWordsByAccuracy { get; set; }
}