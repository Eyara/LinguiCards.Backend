namespace LinguiCards.Application.Common.Models;

public class TrainingWord : WordDto
{
    public Guid TrainingId { get; set; }
    public TrainingType Type { get; set; }
    public List<string> Options { get; set; }
    
    public List<string> ConnectionTargets { get; set; }
    
    public List<WordConnection> ConnectionMatches { get; set; }
}

public class WordConnection
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string TranslatedName { get; set; }
    public TrainingType Type { get; set; }
    public Guid TrainingId { get; set; }
}

public enum TrainingType
{
    FromLearnLanguage = 0,
    FromNativeLanguage,
    WritingFromLearnLanguage,
    WritingFromNativeLanguage,
    WordConnect,
    Sentence
}