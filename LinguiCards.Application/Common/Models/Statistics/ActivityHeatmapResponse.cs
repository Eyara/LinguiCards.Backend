namespace LinguiCards.Application.Common.Models.Statistics;

public class ActivityHeatmapResponse
{
    public List<HeatmapDay> Days { get; set; } = new();
}

public class HeatmapDay
{
    public DateOnly Date { get; set; }
    public int GainedXp { get; set; }
    public int TargetXp { get; set; }
    public bool IsCompleted { get; set; }
    public int ActivityLevel { get; set; }
}
