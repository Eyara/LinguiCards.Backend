namespace LinguiCards.Domain.Entities;

public class IrregularVerb
{
    public int Id { get; set; }
    public string BaseForm { get; set; }
    public string PastSimple { get; set; }
    public string PastParticiple { get; set; }

    public int LanguageId { get; set; }
    public Language Language { get; set; }
}

