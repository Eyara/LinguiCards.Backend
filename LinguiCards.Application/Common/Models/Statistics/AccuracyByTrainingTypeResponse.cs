namespace LinguiCards.Application.Common.Models.Statistics;

public class AccuracyByTrainingTypeResponse
{
    public TrainingAccuracy PassiveWordTraining { get; set; } = new();
    public TrainingAccuracy ActiveWordTraining { get; set; } = new();
    public AggregateAccuracy Grammar { get; set; } = new();
    public AggregateAccuracy TranslationEvaluation { get; set; } = new();
}

public class TrainingAccuracy
{
    public double Accuracy { get; set; }
    public int TotalAttempts { get; set; }
    public int CorrectAttempts { get; set; }
}

public class AggregateAccuracy
{
    public double AverageAccuracy { get; set; }
    public int TotalAttempts { get; set; }
}
