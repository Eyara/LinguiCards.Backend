namespace LinguiCards.Application.Common.Models.Statistics;

public class RetentionCurveResponse
{
    public List<RetentionPeriod> Periods { get; set; } = new();
}

public class RetentionPeriod
{
    public DateTime PeriodStart { get; set; }
    public double Accuracy { get; set; }
    public int TotalAnswers { get; set; }
    public int CorrectAnswers { get; set; }
}
