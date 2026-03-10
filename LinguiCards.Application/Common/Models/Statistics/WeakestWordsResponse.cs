namespace LinguiCards.Application.Common.Models.Statistics;

public class WeakestWordsResponse
{
    public List<WeakWord> Critical { get; set; } = new();
    public List<WeakWord> Weak { get; set; } = new();
    public List<WeakWord> NeedsPractice { get; set; } = new();
}

public class WeakWord
{
    public int WordId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TranslatedName { get; set; } = string.Empty;
    public double Accuracy { get; set; }
    public int TotalAttempts { get; set; }
    public int CorrectAttempts { get; set; }
    public DateTime LastAttempted { get; set; }
    public double PassiveLearnedPercent { get; set; }
    public double ActiveLearnedPercent { get; set; }
}
