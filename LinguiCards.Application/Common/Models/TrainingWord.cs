namespace LinguiCards.Application.Common.Models;

public class TrainingWord : WordDto
{
    public TrainingType Type { get; set; }
    public List<string> Options { get; set; }
}

public enum TrainingType
{
    FromLearnLanguage = 0,
    FromNativeLanguage,
    WritingFromLearnLanguage,
    WritingFromNativeLanguage,
    Sentence
}